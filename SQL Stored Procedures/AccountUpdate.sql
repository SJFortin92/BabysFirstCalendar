USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[AccountUpdate]    Script Date: 4/3/2021 5:47:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 2/21/2021
-- Description:	Updates an account
-- =============================================
ALTER PROCEDURE [dbo].[AccountUpdate](
	@FirstName varchar(50),
	@LastName varchar(50),
	@CurrentEmail varchar(50),
	@NewEmail varchar(50),
	@Phone varchar(20),
	@NotificationSchedule int,
	@Success bit Output,
	@ErrorStatus Nvarchar(50) Output
	)
AS
SET NOCOUNT ON;
BEGIN
	DECLARE
	@TempPersonID int

	
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
		 
			
			 UPDATE Account
				SET FirstName = @FirstName,
					LastName = @LastName,
					Email = @NewEmail,
					PhoneNumber = @Phone,
					NotificationScheduleID = @NotificationSchedule,
					DateLastUsed = SYSDATETIME()
				WHERE Email = @CurrentEmail			
				
				SET @success =1;
				SET @ErrorStatus =0;

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
			  SET @ErrorStatus=  @ErrorStatus + 'Update Person Failed -1001';
			  RAISERROR(@ErrorStatus, 15, 1);
		
			  
		END CATCH
		
	END