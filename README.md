# Twitch Script Runner

This application connects to the Twitch API and executes scripts with specific events are triggered.

Currently supported event triggers:

- Bit donations
- Channel point redemptions

Currently supported script execution types:

- Python (*.py)
- AutoHotKey (*.ahk)

***Important: this app was developed and tested on a Windows 10 machine, and I can make no guarentee to it working on other OS systems***

#### [Please consider sending me a tip!](https://ko-fi.com/timetravelpenguin)

## Prerequirements

- [.NET Core runtime](https://dotnet.microsoft.com/download/dotnet/5.0/runtime) (will prompt on execution if not installed)
- Python (to run python scripts)
- AutoHotKey (to run akh scripts)
- Must have an account on dev.twitch.tv

## Installation

1. Ensure Python and/or AutoHotKey are within the system path. To test this, open the Command Prompt and type "python" and "autohotkey". If an error returns, the variable is not within the path.

   On windows, open the start menu and search for **"Edit the system environment variables"**. Select **Environment Variables** from the **Advanced** tab. Under the section **Sytem variables**, scroll and select the variable **"Path"**, then click **Edit...** from the button bellow. Click **New** and paste the directory containing **python.exe** and **autohotkey.exe**.

2. Run the TwitchScriptRunner.exe for the first time. You will be presented with:

   ```Creating 'appconfig.json'. Please modify the fields within and relaunch the application.```

   Open the ***appconfig.json*** file within a text editor to see the following:

   ```json
   {
     "ApplicationClientId": "dev.twitch.tv/console/apps client id here",
     "ApplicationClientSecret": "dev.twitch.tv/console/apps client secret here",
     "ChannelName": "your channel name",
     "OAuthAccessToken": "get an access token from here https://twitchtokengenerator.com",
     "ApiListeningEvents": {
       "BitDonations": false,
       "ChannelPointRedemption": false
     },
     "ScriptDirectory": {
       "ScriptDirParentPath": "link\\to\\script\\dir"
     }
   }
   ```

   Each of these values must be configured without breaking the structure of the json file.

   ***Note: If you use a path that has back slashes, replace the single backslashes with TWO backslashes. i.e. change '\\' to '\\\\'.***

    - **ApplicationClientId** and **ApplicationClientSecret**: login to and navigate to dev.twitch.tv/console/apps. Select **Register your application** and fill in the fields. 

        - The *Name* field is the name you want this application to be for you to identify it in the future. This isn't too important, so use something like *"TwitchScriptRunner"*.

        - The *OAuth Redirect URLs* field is very important for an upcomming setup step. Input `https://twitchtokengenerator.com` into the field.

        - *Category* should be selected to ***Chat Bot***.

        - Finally, copy the value from **Client ID** and **Client Secret** into the sections **ApplicationClientId** and **ApplicationClientSecret**, respectively. These values are to remain secret and confidential, as they allow you to connect your application to Twitch.

    - **ChannelName**: input the name of your channel so the bot knows what chat to monitor.

    - **OAuthAccessToken**: goto https://twitchtokengenerator.com and scroll down to the section titled **Helix**. You need to select which permissions your application will have.

      Select the options `channel:read:redemptions` and `bits:read`. Ensure they are the only values set to `Yes`, as no other testing has been done with the application and there may be unexpected results.

      Scroll down and click **"Generate Token!"**. You will be redirected to Twitch to authorise the website. Click **Authorize** and you will be returned. After some Captcha validation, copy the generated string within the **Generated Tokens** section, labelled **ACCESS TOKEN**. Paste this value into your json file for **OAuthAccessToken**.

    - **ApiListeningEvents**: change the nested property values to either `true` or `false`. This will indicate what kind of scripts will execute from the events of your stream (i.e. bit donations, channel point redemption).

    - **ScriptDirectory**: this is the directory where the application will look for scripts. It has a very specific file structure. The default value here is **<.exe dir>\\Scripts**. The folder structure should automatically be generated to look like (note I have demo .py and .ahk files bellow -- they won't be present):

      ```console
      .\Scripts
      ├───BitDonations
      │       BitDonations100.py
      │       BitDonations200.ahk
      │
      └───ChannelPointRedemption
              ChannelPointRedemptionCustomReward01.py
              ChannelPointRedemptionCustomReward02.ahk
      ```

3. Add your scripts to the correct directories as shown above. Note that there is a very specific naming system:

   - **BitDonations scripts**: must start with *"BitDonations"* followed by the number of bits (exact amount) needed to run the script. i.e. a 100 bit donation triggers the python script BitDonations100.py

   - **ChannelPointRedemption scripts**: must start with *"ChannelPointRedemption"* followed by the exact name of the reward, with all spaces removed. i.e. ***"My Reward"*** triggers the  python script ***"ChannelPointRedemptionMyReward.py"***.

   ***Important Note: in the future, this system may eventually be changed to link scripts explicitly from the configuration file. As an example:***

   ```json
   {
     "BitDonations": [
       { "100" : "Bits100.py" },
       { "200" : "200Bits.ahk" }
     ],
     "ChannelPointRedemption": [
       { "Redemption01" : "Redemption01Script.py" },
       { "AnotherRedemption" : "AnotherScript.ahk" }
     ]
   }
   ```

   This was not implemented because I honestly didn't think about it until I wrote this document.

4. Run the application! You will see a lot of logging. If there are errors, it will show. ***Do not show this console on stream, as it may contain information for your `ApplicationClientID` and `ApplicationClientSecret`***.
