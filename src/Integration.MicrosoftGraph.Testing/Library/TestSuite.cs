using Integration.MicrosoftGraph.Library.Clients;
using Integration.MicrosoftGraph.Library.Models;
using Newtonsoft.Json;
using Xunit;

namespace Integration.MicrosoftGraph.Testing.Library
{
    public class TestSuite
    {
        private static string tenant = "";
        private static string clientId = "";
        private static string clientSecret = "";
        private static MSGraphClient client = new MSGraphClient(clientId, clientSecret, tenant);
        private static User testUser = new User();

        public TestSuite()
        {
            testUser.displayName = "Test User";
            testUser.mailNickname = "tuser";
            testUser.userPrincipalName = "tuser@shepbopgmail.onmicrosoft.com";
            testUser.givenName = "Test";
            testUser.jobTitle = "Associate";
            testUser.mobilePhone = "555-555-5555";
            testUser.officeLocation = "555 Alabama St, Houston, TX, 77021";
            testUser.preferredLanguage = "en-US";
            testUser.surname = "User";
            testUser.accountEnabled = true;
            testUser.passwordProfile = new PasswordProfile("P@ssword!");
        }

        [Fact]
        public async void T01_GetUsersTest()
        {
            //arrange
            var userJson = JsonConvert.SerializeObject(testUser);
            var creationResponse = client.CreateUser(userJson).Result;

            var usersString = await client.GetUsers("");
            var usersResponse = JsonConvert.DeserializeObject<MSGraphUserListResponse>(usersString);

            //act
            var usersList = usersResponse.value;

            //assert
            Assert.True(usersList.Count >= 1);

            //clean up
            var userString = await client.GetUsers(testUser.userPrincipalName);
            var userResponse = JsonConvert.DeserializeObject<User>(userString);
            var user_id = userResponse.id;
            var deleteResponse = await client.DeleteUser("/users/" + user_id);
        }


        [Fact]
        public async void T02_GetUser()
        {
            //arrange
            var userJson = JsonConvert.SerializeObject(testUser);
            var creationResponse = client.CreateUser(userJson).Result;

            //act
            var userString = await client.GetUsers(testUser.userPrincipalName);
            var userResponse = JsonConvert.DeserializeObject<User>(userString);

            //assert
            Assert.True(testUser.displayName == userResponse.displayName);

            //cleanup
            var user_id = userResponse.id;
            var deleteResponse = await client.DeleteUser("/users/" + user_id);
        }

        [Fact]
        public async void T03_AddUser()
        {
            //arrange
            var userJson = JsonConvert.SerializeObject(testUser);
            var creationResponse = client.CreateUser(userJson).Result;

            //act
            var userString = await client.GetUsers(testUser.userPrincipalName);
            var userResponse = JsonConvert.DeserializeObject<User>(userString);

            //assert
            Assert.True(userResponse.displayName == testUser.displayName);

            //clean up
            var user_id = userResponse.id;
            var deleteResponse = await client.DeleteUser("/users/" + user_id);
        }
    }
}