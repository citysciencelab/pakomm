![PaKOMM-Logo_Claim_Gruen-Blau_Transparent](https://github.com/user-attachments/assets/6bc6db8a-b749-4c30-a01f-2a15d1f63bf1)



https://github.com/user-attachments/assets/a72d3049-8323-47fe-812a-0cf77a1190e9



⚠️ This guide helps you getting started and might require extensive previous knowledge regarding the use of Docker, Unity and its Package Manager (UPM) and Xcode. Also notice that the project is currently no longer being maintained.

For further Information visit the [PaKOMM Website](https://pakomm.de/).

# Table of Content

[Server](#server)

- [Dgraph (23.1.1)](#dgraph-2311)
- Cloud Storage for Firebase (11.6.0) (optional) (for the setup see the [App section](#apps))

[Apps](#apps)

- [Unity (2023.1.20f1)](#unity-2023120f1)
	- [Geodata](#geodata)
	- [Configuring Third Party Plugins and Assets](#configuring-third-party-plugins-and-assets)
		- [SimpleGraphQL-For-Unity (2.1.0)](#simplegraphql-for-unity-210))
		- [Normcore (2.6.1)](#normcore-261)
		- [Touchscript (9.0)](#touchscript-90)
		- [Immersal SDK (1.19.0)](#immersal-sdk-1190)
		- [Cloud Storage for Firebase (11.6.0) (optional)](#cloud-storage-for-firebase-1160-optional)
	- [3D Assets (Editable Objects)](#3d-assets-editable-objects)
	- [Building the Apps](#building-the-apps)
		- [Building for Windows](#building-for-windows)
		- [Building for Android](#building-for-android-meta-quest-2)
		- [Building for iPadOS](#building-for-ipados)

# Server

## Dgraph (23.1.1)

The folder `server` contains PaKOMM's stack for the graph database to be deployed via *Docker Swarm*. In chosen in an agile development approach, might be replaced by geo database in a future development cycle.

1. Create the necessary SSL certificates and provide them via a Docker Bind mount to the Nginx reverse proxy service.
2. Set the Dgraph token `pakomm-stack.yml` file in for the Dgraph `alpha1` service to protect the admin endpoint.

⚠️ Note that the Dgraph token just protects the admin endpoint (.../admin) and is not a secure way to protect the database endpoint (.../graphql). For production grade security, a more advanced setup is required. Alternatively, you can also get a Dgraph Cloud instance, which allows protection of the database endpoint with an *App Key*. 

3. Initialise the database with 

```Shell
curl 'https://xxx:8080/admin' -H 'Accept-Encoding: gzip, deflate, br' -H 'Content-Type: application/json' -H 'Accept: application/json' -H 'Connection: keep-alive' -H 'Origin: altair://-' -H 'X-Dgraph-AuthToken: xxx' --data-binary '{"query":"mutation InitializeSchema {\n  updateGQLSchema(\n    input: {\n      set: {\n        schema: \"\"\"\n            type EventSession @withSubscription {\n          number: Int! @id\n          name: String @search\n          created_at: DateTime\n          screenshotPath: String\n          objects: [PlacedObject] @hasInverse(field: eventSession)\n          brushAnnotation: [BrushAnnotation] @hasInverse(field: eventSession)\n          logOperation: [LogOperation] @hasInverse(field: eventSession)\n          deleted: Boolean\n          locked: Boolean\n        }\n\n        type PlacedObject @withSubscription {\n          id: ID!\n          created_at: DateTime\n          pX: Float\n          pY: Float\n          pZ: Float\n          rX: Float\n          rY: Float\n          rZ: Float\n          sX: Float\n          sY: Float\n          sZ: Float\n          prefabName: String\n          eventSession: EventSession\n          modified_at: DateTime\n          annotation: [Annotation] @hasInverse(field: placedobject)\n        }\n\n        type LogOperation {\n          id: ID!\n          number: Int\n          created_at: DateTime\n          eventSession: EventSession\n          content: String\n          objectid: String\n          pX: Float\n          pY: Float\n          pZ: Float\n          rX: Float\n          rY: Float\n          rZ: Float\n          sX: Float\n          sY: Float\n          sZ: Float\n        }\n\n\n        type BrushAnnotation {\n          id: ID!\n          created_at: DateTime\n          pX: Float\n          pY: Float\n          pZ: Float\n          rX: Float\n          rY: Float\n          rZ: Float\n          sX: Float\n          sY: Float\n          sZ: Float\n          prefabName: String\n          eventSession: EventSession\n          modified_at: DateTime\n          brushStrokeWidth: Float\n          brushColor: BrushColor\n          ribbons: [RibbonPoint]\n          annotation: [Annotation] @hasInverse(field: brushAnnotation)\n        }\n\n        type RibbonPoint {\n          id: ID!\n          number: Int\n          pos: PositionFormat\n          rot: RotationFormat\n        }\n\n        type BrushColor {\n          r: Float\n          g: Float\n          b: Float\n          a: Float\n        }\n\n        type PositionFormat {\n          x: Float\n          y: Float\n          z: Float\n        }\n\n        type RotationFormat {\n          w: Float\n          x: Float\n          y: Float\n          z: Float\n        }\n\n        type Annotation @withSubscription {\n          id: ID!\n          content: String\n          created_at: DateTime\n          placedobject: PlacedObject @hasInverse(field: annotation)\n          brushAnnotation: BrushAnnotation @hasInverse(field: annotation)\n          upvotes: Int\n          downvotes: Int\n        }\n        \"\"\"\n      }\n    }\n  ) {\n    gqlSchema {\n      id\n      schema\n      generatedSchema\n    }\n  }\n}","variables":{}}' --compressed
```

4. You might want to test the successful initialisation of the database with

```shell
curl 'https://xxx:8080/admin' -H 'Accept-Encoding: gzip, deflate, br' -H 'Content-Type: application/json' -H 'Accept: application/json' -H 'Connection: keep-alive' -H 'Origin: altair://-' -H 'X-Dgraph-AuthToken: xxx' --data-binary '{"query":"query getSchema {\n  getGQLSchema {\n    id\n    schema\n    generatedSchema\n  }\n}","variables":{}}' --compressed
```

# Apps

## Unity (2023.1.20f1)
### Geodata

The import pipeline for use of geodata in Unity has been extensively described by [Keil et al. 2021](https://doi.org/10.1007/s42489-020-00069-6). Make sure to choose the layer `15: Landscape` for surfaces that should allow teleporation in VR. 

### Configuring Third Party Plugins and Assets

#### SimpleGraphQL-For-Unity (2.1.0)

[Navid Kabir](https://github.com/NavidK0)'s `SimpleGraphQL-For-Unity` Unity package provides the *GraphQL* client in Unity. For more information and a setup guide see the [README.md](https://github.com/NavidK0/SimpleGraphQL-For-Unity/blob/main/README.md) . ([MIT license](https://github.com/NavidK0/SimpleGraphQL-For-Unity#MIT-1-ov-file))

```
`https://github.com/ngoninteractive/SimpleGraphQL-For-Unity.git`
```
To use the client in Unity, ...
1. add package from git URL to the UPM,
2. past the Dgraph endpoint [created in the server section](#dgraph-2311) (e. g. https://.../graphql) in the asset file (`GraphQLConfig`) located in the folder `Assets/Configs/`, and
3. assign the configuration file to the `DgraphQuery` script assigned to the `GlobalManager` GO (GameObject) in the `BaseScene`.

#### Normcore (2.6.1)

[Normal](https://www.normalvr.com/)'s Normcore provides real-time collaboration between all participants (touch table, VR, and AR). To install and configure the package also see the [Getting Started](https://docs.normcore.io/essentials/getting-started) guide. 

To use the Normcore, ...
1. [register](https://dashboard.normcore.io/register) for a Normcore account,
2. create a Normcore application in the dashboard (in the project phase a [free plan with limitations](https://normcore.io/pricing) is offered)
3. add the package via [Unity's Asset Store](https://assetstore.unity.com/packages/tools/network/normcore-free-multiplayer-voice-chat-for-all-platforms-195224),
4. add Normcore's `Realtime` script to the `NetworkManager` GO in the `BaseScene`, 
5. assign the added script to the `NetworkManager` script in the `NetworkManager`, and
6. past the `App Key` created in step 2. Finally, add Normcore's [RealtimeView](https://docs.normcore.io/realtime/realtimeview) and [RealtimeTransform](https://docs.normcore.io/realtime/realtimetransform) scripts to the assets as described in [3D Assets (Editable Objects)](#3D Assets (Editable Objects)).

#### Touchscript (9.0)

[TouchScript](https://github.com/TouchScript/TouchScript) provides TUIO input functionality used for the marker input on the touch table. To install the library follow the [Getting started](https://github.com/TouchScript/TouchScript?tab=readme-ov-file#getting-started) section. ([MIT license](https://github.com/TouchScript/TouchScript/blob/master/license.txt))

Once installed, ...
1. add the `TouchManager` Prefab to the `BaseScene` and
2. follow the instructions in the [Building the Apps](#building-the-apps) section to chose the appropriate settings when building the app.

#### Immersal SDK (1.19.0)

[Immersal](https://immersal.com/) provides a visual positioning approach for the AR functionality. To install and configure the library also see the [SDK documentation](https://immersal.gitbook.io/sdk) guide. 

To use Immersal, ...
1. register for an [Immersal Developer Portal](https://developers.immersal.com/),
2. download the Immersal SDK 1.19.0 Core (.unitypackage) provided in the portal,
3. download the [immersal-sdk-samples](https://github.com/immersal/immersal-sdk-samples/tree/3a7011550042d41b926db24619bf5bd1b839e4ee) Unity project to use it its `SampleScene` as a reference for the following steps (and to compile the scanning app instead of step 5, if needed),
4. add the `ImmersalSDK` prefab and the `ARSpace` prefab containing `ARMap` GOs. For the real-world registration, 
5. use the app [Immersal](https://apps.apple.com/us/app/immersal/id1361141085) for scanning and download the processed results (.bytes and ...-metadata.json files) from the developer portal. Finally, 
6. import the files in the `Map Data` folder in the Unity project and assign the .bytes file to the `Map File` attribute in the `ARMap` script. (Use a separate GO with an `ARMap` for each scan.)
7. To align the scan with the geodata curated in a dedicated scene (see [Geodata](#Geodata)), load the latter and the `BaseScene` together in the Unit's *Hierarchy* and change the `Transform` component of the `ARMap` GO until the latter's point cloud is properly overlapping with the geodata.

#### Cloud Storage for Firebase (11.6.0) (optional)

[Cloud Storage for Firebase](https://firebase.google.com/docs/storage) provides thumbnail functionality and is used to store the thumbnail screenshots users can take at the touch table or in AR. This package is optional and does not need to be installed if the thumbnail functionality is not required.

To utilize Cloud Storage for Firebase, ...
1. create a Google account and create a new app in the Firebase Console and make sure you understand and setup the [Security & Rules](https://firebase.google.com/docs/storage/security), 
2. download the [FirebaseStorage_11.6.0.unitypackage](https://dl.google.com/firebase/sdk/unity/dotnet4/FirebaseStorage_11.6.0.unitypackage), which also contains the SDK's dependencies,
3. resolve the all needed plaform-specific libraries in the `Assets>External Dependency Manager` menu before building, 
4. download the `google-services.json` and `GoogleService-Info.plist` files and copy them in the `Assets` folder. Finally, 
5. assign a default thumbnail to the `NetworkManager` GO's `ScreenshotHelper` script.

### 3D Assets (Editable Objects)

Import the 3D-Object to Unity and make sure that ...
- textures, materials and shaders are fine (make sure that the shader supports *emission*) and that
- all parts or the parent GO have an appropriate collider.

To use the assets in the scene as editable objects that can be manipulated by the users create a Unity prefab for each item. the prefabs must be located in the `Resources` folder or its child folders to be used with [Normcore (2.6.1)](#normcore-261). 

(1) For the miniature models in the VR Menu copy the prefabs into the folder associated with the desired category (e. g. for a MobilityHub `Assets/Ressources/MobilityHub/`).

(2) For the thumbnails in in the menus of the touch table and the iPads, create thumbnails with a transparent background and save the png files in the `Assets/Ressources/Thumbnails/` folder. Categories are derived from the subfolders in (1).

(3) For the editable objects in the scene, copy the exact same models in the `Assets/Ressources/` folder. These objects will be instantiated during runtime, so they need realtime components (in our case from normcore as described above). Fore the realtime synchronization via [Normcore (2.6.1)](#normcore-261) add the latter's `RealtimeView` and `RealtimeTransform` scripts as components to the prefabs. Furthermore, add the `DatabaseSync` script as well to allow the saving edits in the [Dgraph (23.1.1)](#dgraph-2311) database. (Do not add these three scripts to the prefabs copied to the category subfolders in the previous step!). 

Notice: If the objects are not true to scale, it might be necessary to test and adjust their sizes iteratively. Test if you can add a new object to the scene during runtime. (If you encounter some issues, check your internet connection, your normcore setup, your database setup and if the objects in `Resources` folder have all necessary scripts attached. **Make sure that all the files representing the same object (VR menu (1), thumbnails on the touch devices (2) and the objects placed in the scene (3)) have the same name in all folders.**

If objects in the VR menu or their default size in the scene are too big or too small, adjust the item's scale individual in the relevant folder of (1) or (3).

To create and delete categories apply the necessary changes in the folder structure in (1) and adapt the `VRMenuHolder` prefab in the `Resources` folder (which is also used in the `BaseScene`) accordingly. Copy one of the category-buttons, place it as you wish, change icon and change path to category folder in `Assets/Ressources/`and assign the *Category Item* representing a the object category in the menu as well as the *Category Transform Position* describing the position in the menu. Finally, for the touch menus, search for `Left Menu` GO, adapt the category buttons, change the icon and assign it according to you wish. 

### Use Case specific Settings

To adapt the starting position on the touch devices, in VR, and in AR, apply the changes in the `GlobalManager` GO in the `BaseScene`.

### Building the Apps


#### Building for Windows

Check `Enable TUIO` in the Touchscript settings (Windows → Touchscript → Settings).

Apply the following settings in the `GlobalManager` script:
- [ ] VR Mode
- [ ] VR Simulator
- [x] TUIO Mode
- [ ] Drawing_Enabled
- [ ] Eraser_Enabled
- [ ] Annotations Visible
- [x] Cam Controllers Enabled

Deactivate the `Standard Input` component in the TouchManager GameObject.

#### Building for Android (Meta Quest 2)

Uncheck `Enable TUIO` in the Touchscript settings (Windows → Touchscript → Settings).

Apply the following settings in the GlobalManager script:
- [x] VR Mode
- [ ] VR Simulator
- [ ] TUIO Mode
- [ ] Drawing_Enabled
- [ ] Eraser_Enabled
- [ ] Annotations Visible
- [ ] Cam Controllers Enabled

3. In build settings:  switch platform to android
4. check player settings and enable OpenXR, set Oculus Touch profile for android platform.  

Activate the `Standard Input` component in the TouchManager GameObject.

#### Building for iPadOS

Uncheck `Enable TUIO` in the Touchscript settings (Windows → Touchscript → Settings).

Apply the following settings in the GlobalManager script:
- [ ] VR Mode
- [ ] VR Simulator
- [x] TUIO Mode
- [ ] Drawing_Enabled
- [ ] Eraser_Enabled
- [ ] Annotations Visible
- [x] Cam Controllers Enabled

Activate the `Standard Input` component in the TouchManager GameObject.

You might need to install Firebase's plaform-specific libraries for iOS with the [CocoaPods](https://cocoapods.org/) after exporting the Xcode project in the previous step. Install the pods using the given pod file in the [Xcode](https://developer.apple.com/xcode/) project folder:

```Shell
pod install
```

Open the workspace file in Xcode to build and install the project.
