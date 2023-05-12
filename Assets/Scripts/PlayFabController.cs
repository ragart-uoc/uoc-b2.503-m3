using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

namespace M3
{
    /// <summary>
    /// Class <c>PlayFabController</c> handles the PlayFab API.
    /// </summary>
    public class PlayFabController : MonoBehaviour
    {
        /// <value>Property <c>playFabController</c> represents the PlayFabController instance.</value>
        public static PlayFabController Pfc;

        /// <value>Property <c>userEmail</c> represents the user's e-mail address.</value>
        private string _userEmail;

        /// <value>Property <c>userPassword</c> represents the user's password.</value>
        private string _userPassword;

        /// <value>Property <c>username</c> represents the user's username.</value>
        private string _username;

        /// <value>Property <c>loginPanel</c> represents the login panel.</value>
        public GameObject loginPanel;
        
        /// <value>Property <c>updateStatsButton</c> represents the update stats button.</value>
        public GameObject updateStatsButton; 

        /// <value>Property <c>playerLevel</c> represents the player's level.</value>
        public int playerLevel;

        /// <value>Property <c>gameLevel</c> represents the game's level.</value>
        public int gameLevel;

        /// <value>Property <c>playerHealth</c> represents the player's health.</value>
        public int playerHealth;

        /// <value>Property <c>playerDamage</c> represents the player's damage.</value>
        public int playerDamage;

        /// <value>Property <c>playerHighScore</c> represents the player's high score.</value>
        public int playerHighScore;

        /// <summary>
        /// Method <c>OnEnable</c> is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            if (Pfc == null) {
                Pfc = this;
            }
            else if (Pfc != this) {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

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
            updateStatsButton.SetActive(true);
            
            GetStats();
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
            updateStatsButton.SetActive(true);
            
            GetStats();
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
        
        /// <summary>
        /// Method <c>SetStats</c> sets the user's statistics.
        /// </summary>
        public void SetStats() {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
                    Statistics = new List<StatisticUpdate> {
                        new StatisticUpdate { StatisticName = "PlayerLevel", Value = playerLevel },
                        new StatisticUpdate { StatisticName = "GameLevel", Value = gameLevel },
                        new StatisticUpdate { StatisticName = "PlayerHealth", Value = playerHealth },
                        new StatisticUpdate { StatisticName = "PlayerDamage", Value = playerDamage },
                        new StatisticUpdate { StatisticName = "PlayerHighScore", Value = playerHighScore }
                    }
                },
                result => { Debug.Log("User statistics updated"); },
                error => { Debug.LogError(error.GenerateErrorReport()); });
        }
        
        /// <summary>
        /// Method <c>GetStats</c> gets the user's statistics.
        /// </summary>
        public void GetStats()
        {
            PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest(),
                OnGetStatistics,
                error => Debug.LogError(error.GenerateErrorReport()));
        }

        /// <summary>
        /// Method <c>OnGetStatistics</c> is called when the user's statistics are received.
        /// </summary>
        /// <param name="result">The result of the statistics.</param>
        private void OnGetStatistics(GetPlayerStatisticsResult result)
        {
            Debug.Log("Received the following Statistics:");
            foreach (var eachStat in result.Statistics)
            {
                Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
                switch (eachStat.StatisticName)
                {
                    case "PlayerLevel":
                        playerLevel = eachStat.Value;
                        break;
                    case "GameLevel":
                        gameLevel = eachStat.Value;
                        break;
                    case "PlayerHealth":
                        playerHealth = eachStat.Value;
                        break;
                    case "PlayerDamage":
                        playerDamage = eachStat.Value;
                        break;
                    case "PlayerHighScore":
                        playerHighScore = eachStat.Value;
                        break;
                }
            }
        }
        
        /// <summary>
        /// Method <c>OnCloudUpdateStats</c> is called when the user's statistics are updated.
        /// </summary>
        public void StartCloudUpdatePlayerStats() {
            PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdatePlayerStats",
                FunctionParameter = new {
                    Level = playerLevel,
                    highScore = playerHighScore,
                    Health = playerHealth
                    
                }, GeneratePlayStreamEvent = true,
            }, OnCloudUpdateStats, OnErrorShared);
        }
        
        /// <summary>
        /// Method <c>OnCloudUpdateStats</c> is called when the user's statistics are updated.
        /// </summary>
        /// <param name="result">The result of the statistics.</param>
        private static void OnCloudUpdateStats (ExecuteCloudScriptResult result) {
            Debug.Log(PluginManager.GetPlugin<ISerializerPlugin>
                (PluginContract.PlayFab_Serializer).SerializeObject (result.FunctionResult));
            var jsonResult = (JsonObject) result.FunctionResult;
            jsonResult.TryGetValue ("messageValue", out var messageValue);
            Debug.Log ((string) messageValue);
        }
 
        /// <summary>
        /// Method <c>OnErrorShared</c> is called when there is an error.
        /// </summary>
        /// <param name="error">The error.</param>
        private static void OnErrorShared (PlayFabError error) {
            Debug.Log (error.GenerateErrorReport());
        }
    }
}
