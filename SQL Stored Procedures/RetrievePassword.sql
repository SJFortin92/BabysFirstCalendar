USE [NewbornCalendarModel]
GO
/****** Object:  StoredProcedure [dbo].[RetrievePassword]    Script Date: 4/3/2021 5:48:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sarai Fortin
-- Create date: 2/14/2021
-- Description:	Pulls the hashed/salted password from the Password table
--				where the email = the account ID
-- =============================================
ALTER PROCEDURE [dbo].[RetrievePassword](
	@Username varchar(150)
	)
AS
SET NOCOUNT ON;
BEGIN
	SELECT P.Password FROM Password as P
	JOIN Account as A
	ON A.AccountID = P.AccountID
	WHERE @Username = A.Email
END