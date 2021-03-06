CREATE OR ALTER TRIGGER set_null_in_UserToFriends_after_deleting_from_AspNetUsers
ON [dbo].[AspNetUsers]
AFTER DELETE AS
BEGIN
	UPDATE [dbo].[UserToFriends] SET FriendId = NULL WHERE FriendId = (SELECT Id FROM deleted)		
END
