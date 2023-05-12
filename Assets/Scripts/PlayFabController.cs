using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace M3
{
    /// <summary>
    /// Class <c>PlayFabController</c> handles the PlayFab API.
    /// </summary>
    public class PlayFabController : MonoBehaviour
    {
        /// <value>Property <c>userEmail</c> represents the user's e-mail address.</value>
        private string _userEmail;

        /// <value>Property <c>userPassword</c> represents the user's password.</value>
        private string _userPassword;

        /// <value>Property <c>username</c> represents the user's username.</value>
        private string _username;
        
        /// <value>Property <c>loginPanel</c> represents the login panel.</value>
        public GameObject loginPanel; 

        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
        /// </summary>
        private void Start()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) {
                PlayFabSettings.staticSettings.TitleId = "BD88C";
            }
            if (PlayerPrefs.HasKey("EMAIL")) {
                _userEmail = PlayerPrefs.GetString("EMAIL");
                _userPassword = PlayerPrefs.GetString("PASSWORD");
                var request = new LoginWithEmailAddressRequest {
                    Email = _userEmail,
                    Password = _userPassword
                };
                PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
                Debug.Log(_userEmail + " user logged in automatically.");
            }
        }

        /// <summary>
        /// Method <c>OnLoginSuccess</c> is called when the login was successful.
        /// </summary>
        /// <param name="result">The result of the login.</param>
        private void OnLoginSuccess(LoginResult result)
        {
            PlayerPrefs.SetString("EMAIL", _userEmail);
            PlayerPrefs.SetString("PASSWORD", _userPassword);
            Debug.Log("Storing " + _userEmail + " credentials into Player Preferences.");
            
            loginPanel.SetActive(false);
        }

        /// <summary>
        /// Method <c>OnLoginFailure</c> is called when the login failed.
        /// </summary>
        /// <param name="error">The error of the login.</param>
        private void OnLoginFailure(PlayFabError error)
        {
            Debug.Log("User " + _userEmail + " does not exist. Registering new player...");
            var registerRequest = new RegisterPlayFabUserRequest
            {
                Email = _userEmail,
                Password = _userPassword,
                Username = _username
            };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        }

        /// <summary>
        /// Method <c>OnRegisterSuccess</c> is called when the registration was successful.
        /// </summary>
        /// <param name="result">The result of the registration.</param>
        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log("Congratulations, new user has been registered!");

            PlayerPrefs.SetString("EMAIL", _userEmail);
            PlayerPrefs.SetString("PASSWORD", _userPassword);
            Debug.Log("Storing " + _userEmail + " credentials into Player Preferences.");
            
            loginPanel.SetActive(false);
        }

        /// <summary>
        /// Method <c>OnRegisterFailure</c> is called when the registration failed.
        /// </summary>
        /// <param name="error">The error of the registration.</param>
        private void OnRegisterFailure(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        
        /// <summary>
        /// Method <c>GetUserEmail</c> gets the user's e-mail address.
        /// </summary>
        /// <param name="emailIn">The user's e-mail address.</param>
        public void GetUserEmail(string emailIn) {
            _userEmail = emailIn;
        }
 
        /// <summary>
        /// Method <c>GetUserPassword</c> gets the user's password.
        /// </summary>
        /// <param name="passwordIn">The user's password.</param>
        public void GetUserPassword(string passwordIn) {
            _userPassword = passwordIn;
        }
 
        /// <summary>
        /// Method <c>GetUsername</c> gets the user's username.
        /// </summary>
        /// <param name="usernameIn">The user's username.</param>
        public void GetUsername(string usernameIn) {
            _username = usernameIn;
        }
        
        /// <summary>
        /// Method <c>OnClickLogin</c> is called when the user clicks the login button.
        /// </summary>
        public void OnClickLogin() {
            var request = new LoginWithEmailAddressRequest {
                Email = _userEmail,
                Password = _userPassword
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
    }
}
