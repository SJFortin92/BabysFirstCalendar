USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[NoteUpdate]    Script Date: 4/3/2021 5:48:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 3/28/2021
-- Description:	Updates a note in the Note table. If there is a photo attached,
--				it inserts the photo reference location and size into the database
-- =============================================
ALTER PROCEDURE [dbo].[NoteUpdate](
	@NoteID int,
	@ChildID int,
	@DateOccurred date,
	@Text varchar(300),
	@HasPhoto bit,
	@PhotoLocation varchar(200),
	@PhotoSize int,
	@Success bit OUTPUT,
	@ErrorStatus varchar(50) OUTPUT
	)
AS
SET NOCOUNT ON;
BEGIN
	DECLARE
	@TempNoteID int,
	@TempPhotoID int,
	@OriginalHasPhoto int,
	@NewPhotoID int,
	@AccountID int;
	
	
	SET @TempNoteID =0;
	SET @TempNoteID =0;
	SET @NewPhotoID =0;
	SET @AccountID=0;
	SET @Success = 0;
	SET @ErrorStatus ='';

	
	           
BEGIN TRY
	 BEGIN TRANSACTION;
	--Select the AccountID, as we will need this to update our accounts later
	SELECT @AccountID = A.AccountID FROM Account AS A
	JOIN Child as C
	ON A.AccountID = C.AccountID
	WHERE C.ChildID = @ChildID

	--Check to make sure the note exists
	BEGIN
		SELECT @TempNoteID = NoteID from Note
		WHERE @NoteID = NoteID
		IF @TempNoteID =0 OR @TempNoteID IS NULL
			BEGIN
				SET @Success =0;
				SELECT @ErrorStatus = 'Note does not exist';
				RAISERROR(@ErrorStatus,16,1);
			END
	END	
	
	--Check to see if they originally had a photo uploaded
	BEGIN
		SELECT @OriginalHasPhoto = HasPhoto from Note
		WHERE @NoteID = NoteID
	END

	--Check the original PhotoID, if it exists
	BEGIN
		SELECT @TempPhotoID = PhotoID FROM Photo
		WHERE @NoteID = NoteID
	END

		IF @@Error <> 0 
			BEGIN
				SELECT @ErrorStatus = CONVERT(nVarchar(50),@@ERROR) + '-1000';
				SET @Success = 0;
			END

	--If this note has a photo, and had a photo before
	 IF @HasPhoto=1 AND @OriginalHasPhoto = 1
		BEGIN
			UPDATE Note
			SET ChildID = @ChildID,
				Date = @DateOccurred,
				Text = @Text,
				HasPhoto = @HasPhoto
			WHERE @NoteID = NoteID

			--Update into the photo table as well
			UPDATE Photo
			SET PhotoLocationReference = @PhotoLocation,
				PhotoSize = @PhotoSize
			WHERE @TempPhotoID = PhotoID
		END

	--If they uploaded a photo and didn't previously
	IF @HasPhoto=1 AND @OriginalHasPhoto = 0
		BEGIN
			UPDATE Note
			SET ChildID = @ChildID,
				Date = @DateOccurred,
				Text = @Text,
				HasPhoto = @HasPhoto
			WHERE @NoteID = NoteID

			--Insert the new photo into the photo table
			INSERT Photo(NoteID, PhotoLocationReference, PhotoSize)
			VALUES (@NoteID, @PhotoLocation, @PhotoSize)
			SET @NewPhotoID = @@IDENTITY
		END
	
	--If they had a photo before but don't now
	IF @HasPhoto = 0 AND @OriginalHasPhoto = 1
		BEGIN
			UPDATE Note
			SET ChildID = @ChildID,
				Date = @DateOccurred,
				Text = @Text,
				HasPhoto = @HasPhoto
			WHERE @NoteID = NoteID

			DELETE FROM Photo
			WHERE @TempPhotoID = PhotoID
		END

	--Otherwise, we just update the text into the note table
	ELSE
		BEGIN
			UPDATE Note
			SET ChildID = @ChildID,
				Date = @DateOccurred,
				Text = @Text,
				HasPhoto = @HasPhoto
			WHERE @NoteID = NoteID
		END

	--Update the DateLastUsed in the Account table
	UPDATE Account
		SET DateLastUsed = SYSDATETIME()
	WHERE Account.AccountID = @AccountID
		
		SET @Success = 1;
		SET @ErrorStatus = 0;

		COMMIT TRANSACTION;
	
		END TRY

		BEGIN CATCH
			  ROLLBACK TRANSACTION;		
			  --We are ignoring @@Error because it returns 50000 whenever a string is passed to RAISERROR
				--We only care if the error was not raised by SSE
				IF @@ERROR <> 50000
					BEGIN
						SELECT @ErrorStatus = @ErrorStatus + CONVERT(nVarchar(50), @@Error);
					END
			  SET @Success =0;
			  SET @ErrorStatus=  @ErrorStatus + '-1001';
			  RAISERROR(@ErrorStatus, 15, 1);
		END CATCH
		
	END