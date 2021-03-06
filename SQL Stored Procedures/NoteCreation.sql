USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[NoteCreation]    Script Date: 4/3/2021 5:47:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 2/12/2021
-- Description:	Inserts a note into the Note table. If there is a photo attached,
--				it inserts the photo reference location and size into the database
-- =============================================
ALTER PROCEDURE [dbo].[NoteCreation](
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
	@NewPhotoID int,
	@NewNoteID int,
	@AccountID int;
	
	
	SET @TempNoteID =0;
	SET @NewNoteID =0;
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

	--If this note has a photo, we run a different insertion process
	 IF @HasPhoto=1
		BEGIN
			INSERT Note(ChildID, Date, Text, HasPhoto)
			VALUES (@ChildID, @DateOccurred, @Text, @HasPhoto)
			SET @NewNoteID = @@IDENTITY

			--Insert into the photo table as well
			INSERT Photo(NoteID, PhotoLocationReference, PhotoSize)
			VALUES (@NewNoteID, @PhotoLocation, @PhotoSize)
			SET @NewPhotoID = @@IDENTITY
		END

	--Otherwise, we just insert the text into the note table
	ELSE
		BEGIN
			INSERT Note(ChildID, Date, Text, HasPhoto)
			VALUES (@ChildID, @DateOccurred, @Text, @HasPhoto)
			SET @NewNoteID = @@IDENTITY
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