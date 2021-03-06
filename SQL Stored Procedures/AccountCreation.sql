USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[AccountCreation]    Script Date: 4/3/2021 5:46:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 2/6/2021
-- Description:	Insert into Password and Account tables
-- =============================================
ALTER PROCEDURE [dbo].[AccountCreation](
	@FirstName varchar(50),
	@LastName varchar(50),
	@Password varchar(MAX),
	@Email varchar(50),
	@NotificationSchedule int,
	@Success bit OUTPUT,
	@ErrorStatus varchar(50) OUTPUT
	)
AS
SET NOCOUNT ON;
BEGIN
	DECLARE
	@TempPersonID int,
	@PersonID int;
	
	
	SET @TempPersonID =0;
	SET @Success = 0;
	SET @ErrorStatus ='';
	SET @TempPersonID =0;		

	
	           
BEGIN TRY
	 BEGIN TRANSACTION;
	
	 SELECT @TempPersonID = AccountID FROM Account
	     WHERE Email =@Email
		IF @TempPersonID <>0
			BEGIN
				SELECT @ErrorStatus = 'Email already in use.';
				SET @Success = 0;
				RAISERROR(@ErrorStatus,16,1);
			END
		
		IF @@Error <> 0 
			BEGIN
				SELECT @ErrorStatus = @ErrorStatus + CONVERT(nVarchar(50), @@Error);
				SET @Success = 0;
				RAISERROR(@ErrorStatus,16,1);
			END
		 
			
			  INSERT Account(FirstName, LastName, PhoneNumber, Email, NotificationScheduleID, DateLastUsed, DataUsed, AccountType)
			  VALUES (@FirstName, @LastName, NULL, @Email, @NotificationSchedule, SYSDATETIME(), 0, 0)
				
				SET @PersonID =@@IDENTITY;

		BEGIN
			INSERT Password(AccountID,Password)
			VALUES(@PersonID,@Password)
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