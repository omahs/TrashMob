﻿
namespace TrashMob.Shared.Managers
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using User = Models.User;

    public class ActiveDirectoryManager : IActiveDirectoryManager
    {
        private readonly IUserManager userManager;

        public ActiveDirectoryManager(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ActiveDirectoryResponseBase> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default)
        {
            User originalUser;

            if ((originalUser = await userManager.UserExistsAsync(activeDirectoryNewUserRequest.identities[0].issuerAssignedId, cancellationToken).ConfigureAwait(false)) != null)
            {
                originalUser.Email = activeDirectoryNewUserRequest.email;
                originalUser.SourceSystemUserName = activeDirectoryNewUserRequest.identities[0].issuerAssignedId;

                // User does exist, see if they are trying to change their userName to something already in use
                if (activeDirectoryNewUserRequest.displayName != originalUser.UserName)
                {
                    var checkUserName = await userManager.GetUserByUserNameAsync(activeDirectoryNewUserRequest.displayName, CancellationToken.None);

                    if (checkUserName != null)
                    {
                        var duplicateDisplayNameResponse = new ActiveDirectoryValidationFailedResponse
                        {
                            action = "ValidationError",
                            version = "1.0.0",
                            userMessage =  "Please choose a different Display Name. This name already in use." 
                        };

                        return duplicateDisplayNameResponse;
                    }
                }

                await userManager.UpdateAsync(originalUser, cancellationToken).ConfigureAwait(false);

                var userExistsResponse = new ActiveDirectoryContinuationResponse
                {
                    action = "Continue",
                    version = "1.0.0",
                };

                return userExistsResponse;
            }

            var checkUser = await userManager.GetUserByUserNameAsync(activeDirectoryNewUserRequest.displayName, CancellationToken.None);

            if (checkUser != null)
            {
                var duplicateDisplayNameResponse = new ActiveDirectoryValidationFailedResponse
                {
                    action = "ValidationError",
                    version = "1.0.0",
                    userMessage = "Please choose a different Display Name. This name already in use."
                };

                return duplicateDisplayNameResponse;
            }

            var user = new User
            {
                Email = activeDirectoryNewUserRequest.email,
                GivenName = activeDirectoryNewUserRequest.givenName,
                SurName = activeDirectoryNewUserRequest.surname,
                UserName = activeDirectoryNewUserRequest.displayName
            };

            await userManager.AddAsync(user, cancellationToken).ConfigureAwait(false);

            var newUserResponse = new ActiveDirectoryContinuationResponse
            {
                action = "Continue",
                version = "1.0.0",
            };

            return newUserResponse;
        }
    }
}