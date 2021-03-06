USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[ChildCreation]    Script Date: 4/3/2021 5:47:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 2/12/2021
-- Description:	Insert a new child into Child table
-- =============================================
ALTER PROCEDURE [dbo].[ChildCreation](
	@AccountID int,
	@FirstName varchar(50),
	@LastName varchar(50),
	@DOB date,
	@Success bit OUTPUT,
	@ErrorStatus varchar(50) OUTPUT
	)
AS
SET NOCOUNT ON;
BEGIN
	DECLARE
	@TempChildID int,
	@NewChildID int;
	
	
	SET @TempChildID =0;
	SET @NewChildID =0;
	SET @Success = 0;
	SET @ErrorStatus ='';

	
	           
BEGIN TRY
	 BEGIN TRANSACTION;
	
	 SELECT @TempChildID = ChildID FROM Child
	     WHERE FirstName = @FirstName AND DateOfBirth = @DOB AND
	           AccountID = @AccountID
		IF @TempChildID <>0
			BEGIN
				SELECT @ErrorStatus = 'Child already exists.';
				SET @Success = 0;
				RAISERROR(@ErrorStatus,16,1);
			END
		
		IF @@Error <> 0 
			BEGIN
				SELECT @ErrorStatus = @ErrorStatus + CONVERT(nVarchar(50), @@Error);
				SET @Success = 0;
				RAISERROR(@ErrorStatus,16,1);
			END
		 
			
			  INSERT Child(AccountID, FirstName, LastName, DateOfBirth)
			  VALUES (@AccountID, @FirstName, @LastName, @DOB)
				
				SET @NewChildID =@@IDENTITY;

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