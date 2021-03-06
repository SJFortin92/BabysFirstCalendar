USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[AccountDeletion]    Script Date: 4/3/2021 5:47:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 2/28/2021
-- Description:	Deletes an account entirely from the Account Table,
--				Password table, all Child rows, all Note rows, and Photo rows
-- =============================================
ALTER PROCEDURE [dbo].[AccountDeletion](
	@CurrentEmail varchar(50),
	@Success bit OUTPUT,
	@ErrorStatus varchar(50) OUTPUT
	)
AS
SET NOCOUNT ON;
BEGIN
	DECLARE
	@TempPersonID int,
	@TempChildID int;
	
	
	SET @TempPersonID =0;
	SET @Success = 0;
	SET @ErrorStatus ='';
	SET @TempPersonID =0;		

	
	           
BEGIN TRY
	 BEGIN TRANSACTION;
	
		BEGIN
			SELECT @TempPersonID = AccountID from Account
				WHERE
					Email = @CurrentEmail
			IF @TempPersonID =0 OR @TempPersonID IS NULL
				BEGIN
					SET @Success =0;
					SELECT @ErrorStatus = 'Account does not exist';
					RAISERROR(@ErrorStatus,16,1);
				END
		END	
		
		IF @@Error <> 0 
			BEGIN
				SELECT @ErrorStatus = CONVERT(nVarchar(50),@@ERROR) + '-1000';
				SET @Success = 0;
			END
		BEGIN
			SELECT @TempChildID = ChildID from Child
				WHERE Child.AccountID = @TempPersonID
		END

		BEGIN

			DELETE p
			FROM Photo p
			INNER JOIN Note n
			on p.NoteID = n.NoteID
			INNER JOIN Child c
			on n.ChildID = c.ChildID
			WHERE c.ChildID = @TempChildID

			DELETE n 
			FROM Note n
			INNER JOIN Child c
			on n.ChildID = c.ChildID
			WHERE c.ChildID = @TempChildID

			DELETE FROM Child
			WHERE ChildID = @TempChildID

			DELETE FROM Password
			WHERE AccountID = @TempPersonID

			DELETE from Account
			WHERE AccountID = @TempPersonID

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