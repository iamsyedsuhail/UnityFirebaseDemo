# UnityFirebaseDemo

This repo helps you understand how firebase service is setup for unity, and use it.

## Scenario
We have a MMO game, where players enter daily, create an item and share it with other players online, the others can view the list of items.
Also delete them, because why not, its a brutal world.

## We will be focusing on using,
- Firebase Auth, for user authentication and profiles.
- Firebase Realtime Database, to store item information.
- Firebase Storage, to retreive asset bundle and load gameobject from it.

## Prerequisite
- Unity Engine 2021.3.0f1, you can download it from [here](https://unity3d.com/get-unity/download).
- Firebase Unity SDK 9.4.0, you can download it from [here](https://github.com/firebase/firebase-unity-sdk/releases).

## Firebase Setup
### General Setup
- Open firebase console, click on **Add Project**.
- Name your project, click next, we dont need analytics for this demo, just disable and click on **Create**.
- Click **Project Overview** on left panel, and select unity, from the home screen to create unity project.
- For this demo,i will just use android, uncheck Register Apple App, select Android App and provide app bundle id and name, click Register.

### Firebase Realtime Database Setup
- On the left panel, under click ***Builds->Realtime Database***, on the right panel, click create database.
- Select your preferred region, then select start on test mode. 
- Now once database is setup, you will notice menus at top ```Data, Rules, Backups, Usage```, select Rules, and change the rules to just accept anyone to write and read
for this demo, you must change this rules later.

```
{
  "rules": {
    ".read": "true",  // change to true
    ".write": "true",  // change to true
  }
}
```

### Firebase Storage Setup
- On the left panel, under click ***Builds->Storage***, on the right panel, click create storage.
- Select your preferred region, then select start on test mode. 
- Now once storage is setup, you will notice menus at top ```Data, Rules, Backups, Usage```, select Rules, and change the rules to just accept anyone to write and read
for this demo, you must change this rules later.

```
rules_version = '2';
service firebase.storage {
  match /b/{bucket}/o {
    match /{allPaths=**} {
      allow read, write: if true;//change it always return true
    }
  }
}
```

### Firebase Authentication
- On the left panel, under click ***Builds->Authentication***, on the right panel, click create storage.
- Under Providers Select Email/Password and click next, again enable Email/Password and click save under Sign-in Providers.

Thats it!.

## Unity Setup
### Open the project in unity
- You will get this error, just click on **Ignore**.

![error](https://github.com/iamsyedsuhail/UnityFirebaseDemo/blob/develop/ReadMeImages/Firebase1.JPG?raw=true)

- Just Opening this project will throw loads of Error!!, but dont worry all these errors are basically due to missing firebase-unity-sdk
- download the sdk from above link and unzip, them **we basically need only 3 packages from dotnet4 folder**.
  - FirebaseAuth.unitypackage
  - FirebaseDatabase.unitypackage
  - FirebaseStorage.unitypackage
  Import them all one by one.
  Once you are done, clear the console, atleast now there shouldn't be any errors.
- Make sure the scripting order, is intact, as its intended by me, i am making sure firebasecontroller is loaded first, you can change this any case as you wish.

![scripting order](https://github.com/iamsyedsuhail/UnityFirebaseDemo/blob/develop/ReadMeImages/Firebase2.JPG?raw=true)

- Open SampleScene, click on **LoadAssetBundle** Gameobject, make sure the scrip it is referenced, if not just add the missing script, this fix Load bundle issue.

![loadbundle](https://github.com/iamsyedsuhail/UnityFirebaseDemo/blob/develop/ReadMeImages/Firebase3.JPG?raw=true)

- Download the ```google-services.json``` which you can find under project settings of your firebase console and put it under ```StreamingAssets``` folder in unity.

![streamingassets](https://github.com/iamsyedsuhail/UnityFirebaseDemo/blob/develop/ReadMeImages/Firebase4.JPG?raw=true)

- In order to use firebase database service, we need to make one more small change, open the downloadeded ```google-services.json``` and add ```"firebase-url"``` under
```"project-info"``` and save, close, **dont edit other details as it will be already updated**.
```
  "project_info": {
    "project_number": "your project number from setting",
    "firebase_url": "your project databse link from realtime database page",
    "project_id": "your project id from setting",
    "storage_bucket": "your project storage link from storage page"
  },
```

![streamingassets](https://github.com/iamsyedsuhail/UnityFirebaseDemo/blob/develop/ReadMeImages/Firebase5.JPG?raw=true)

![streamingassets](https://github.com/iamsyedsuhail/UnityFirebaseDemo/blob/develop/ReadMeImages/Firebase6.JPG?raw=true)

DONE!!!!! this was just the setup, try the game, read through the code, and explore yourself to understand things, **there is no better teacher than yourself!**.

## More
- Learn more about creating asset bundles, from my [UnityAssetBundle](https://github.com/iamsyedsuhail/UnityAssetBundle) Repo.
- Read more about firebase, check out their [documentation for unity](https://firebase.google.com/docs/build).

