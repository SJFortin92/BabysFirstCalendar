USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[NoteDeletion]    Script Date: 4/3/2021 5:48:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 3/28/2021
-- Description:	Deletes a note
-- =============================================
ALTER PROCEDURE [dbo].[NoteDeletion](
	@NoteID int,
	@Success bit OUTPUT,
	@ErrorStatus varchar(50) OUTPUT
	)
AS
SET NOCOUNT ON;
BEGIN
	 DECLARE @TempNoteID int,
			@TempPhotoID int
	 
	 SET @TempNoteID = 0;
	 SET @TempPhotoID = 0;

BEGIN TRY
	 BEGIN TRANSACTION;
		
		BEGIN
			--Checks if the note exists in the database
			SELECT @TempNoteID = NoteID from Note
			WHERE @NoteID = NoteID
			IF @TempNoteID =0 OR @TempNoteID IS NULL
				BEGIN
					SET @Success =0;
					SELECT @ErrorStatus = 'Note does not exist';
					RAISERROR(@ErrorStatus,16,1);
				END
			--Checks if a photo is associated with the note
			SELECT @TempPhotoID = PhotoID from Photo
			WHERE @NoteID = NoteID
		END	
		
		IF @@Error <> 0 
			BEGIN
				SELECT @ErrorStatus = CONVERT(nVarchar(50),@@ERROR) + '-1000';
				SET @Success = 0;
			END

		--Delete the note
		BEGIN

		IF @TempPhotoID <> 0 OR @TempPhotoID IS NOT NULL
			BEGIN
				DELETE FROM Photo
				WHERE @TempPhotoID = PhotoID

				DELETE FROM Note
				WHERE @NoteID = NoteID
			END

		ELSE
			BEGIN
				DELETE FROM Note
				WHERE @NoteID = NoteID
			END
		END

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