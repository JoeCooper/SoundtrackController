# Soundtrack Controller
A background music controller for Unity3D using IoC; SoundtrackController observes project-specific "filters" to learn what music should play, and manages transitions.

AudioClips are not referenced directly, but instead are accessed through "Music Track Configuration" objects which exist in a scene hierarchy and couple a reference to an AudioClip with other metadata.

A description of our usage scenario, including the embedding strategy, appears at the end of this readme.
## Namespace
All classes are kept under the namespace NobleMuffins.SoundtrackPlus.

## The Soundtrack Controller prefab
Place this into the first scene that is loaded, or into any scene where you want the Soundtrack Controller to work.

No further configuration of this object is necessary.

This object carries a single MusicPlaybackController component. This component is a "singleton", which here means that only one will exist in the process. It will not create itself and must be placed, at least, into the first scene. If one is placed into every scene – you might want this for testing purposes – than new instances of MusicPlaybackController will destroy themselves if they find that a MusicPlaybackController is already present.
## Music Track Configuration
A Music Track Configuration couples a reference to an AudioClip with other metadata wanted by the soundtrack controller.

To create a new music track configuration in your project hierarchy, choose "Music Track Configuration" from the Create menu in your project hierachy window.

Currently, only one additional field is present; "Transition Type".
### Transition Type

If track A is currently playing, and we wish to transition to track B, than track B's "transition type" describes how this transition will occur. The types are as follows:

* FadePriorAndStart – The previous track will fade out before this track begins to play.
* Fade – Smoothly fade from the previous track to this one.
* HaltPriorAndStart – Any track currently playing will be halted immediately so that this track can start.

## class AbstractMusicFilter { … }
This class features an abstract method – CurrentMusicTrack { get; } – and one field; priority.

In order to learn which music track should be currently playing, MusicPlaybackController will poll the set of AbstractMusicFilters currently present in the scene. The filter that has both returns a non-null CurrentMusicTrack and has the highest priority decide what music track will play.

In order to control the music, you must create a project-specific subclass of AbstractMusicFilter and that subclass – or those subclasses – must answer the question "what music should currently be playing?"

Any number of these filters can exist on the scene at one time, and they can be persistent or not persistent, and can be both created and destroyed at runtime.

However, if one or more music filters exist simultaneously *with the same priority*, you will get unpredictable results.

## SingleTrackMusicFilter

A very basic implementation of AbstractMusicFilter.

This filter has a public field where you can assign a music track, and it will always return that.

## Our case
We're using this in a current project. I'll describe here each key point.

### Hierarchial Music Decisions

In a typical scene, either two or three music filters are used.

Attached to the player character is a "Hero Music Filter" which can map the user's location to a currently preferred music track. One room might be associated with music track X, which another is associated with music track Y.

In the scene itself is a separate music filter called ScriptControlledMusicFilter which has a **higher priority** than the Hero Music Filter, and provides a static interface where a cutscene script can request a particular track, request silence, or *relinquish control* so that authority is returned to the Hero Music Filter. (Relinquishing control is done simply by returning null for CurrentMusicTrack.) ScriptControlledMusicFilter.

### Simpler Scenes

In scenes which do not require sophisticated music selection, like the Map scene, we simply add a SingleTrackMusicFilter to that scene. Because the music playback controller persists between scenes, the music will transition as desired when a new scene is loaded.

### Embedding

Since the project is kept in a git repository, we can add Soundtrack Controller as a git submodule under Assets.

### Disentanglement

Scenes and scripts do not keep references to particular AudioClips, but instead refer to Music Track Configurations which are named by intent. One MTC might, for example, have the name "Desert Dungeon Exterior", and if at any point I want to change the audioclip used for that situation, I change it in the MTC, which exists only once in the project. References in scenes do not need to be altered.