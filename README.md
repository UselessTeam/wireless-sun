# Wireless Sun Project
The combination of [WirelessRPG](https://github.com/UselessTeam/WirelessRPG) and [LawlessIsland](https://github.com/UselessTeam/lawless-island), made with [Godot](https://godotengine.org/).

## Running the game

The repo uses [git lfs](https://git-lfs.github.com/) to store images and sounds, so start by installing it on your computer. Then you can clone the repository with
```
git lfs clone git@github.com:UselessTeam/wireless-sun.git
```

You need to install the [Godot 3.2.1, Mono Version](https://godotengine.org/download/). It requires MSBuild, so make sure to follow all the steps on the page.
You should be able to call the following command with no error:
```
msbuild -version
```

Then, you need to download and add the C# packages to the project. You can execute the following command at the root of the project to do so.
```
nuget restore
```

Finally, open the project through the Godot editor. You should be able to run it by pressing 'F5', or clicking on the triangle on the top right.
