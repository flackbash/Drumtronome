# Drumtronome #

Drumtronome is a metronome not only, but especially for drummers.

![running](https://raw.github.com/flackbash/Drumtronome/master/Screenshots/running.png "Drumtronome running")

Drumtronome has the usual functionalities of a simple metronome, but there's much more to it than that!
Have you ever been annoyed by having to stop your paradiddles or rudiments in order to increase the tempo? This metronome allows you to specify tempo adjustments *before* you start playing. In addition to a simple speed trainer which will automatically increase the tempo after a certain number of bars, Drumtronome has a unique speed template generator which enables you to create your very own, personalized tempo patterns and save them for later sessions.

![speedTemplate](https://raw.github.com/flackbash/Drumtronome/master/Screenshots/speedTemplate.png "The Speed Template HUD")


## Use ##

Most things are self-explanatory - the things that are not are explained in this section.

![use](https://raw.github.com/flackbash/Drumtronome/master/Screenshots/use.png "The usage of Drumtronome")

###### The HUDs ######

Every HUD has an *OK* (check mark) and a *Cancel* (cross) button. As you probably guessed, pressing the first will save your changes and close the HUD while the latter will simply close the HUD and restore the previous settings. You can achieve the same result by pressing your Enter or Escape key respectively.

###### The Speed Template language ######

In order to create your own speed templates you have to know a few things about the speed template language.

Each template consists of a number of statements (number > 0).
A statement starts with a number that specifies the amount of bars, followed by an *x* and another number that specifies the beats per minute.
Each statement ends with a dot. 

So for example a statement could look like this: `4x100.`
In this case the metronome would simply play 4 bars at 100 bpm.

When playing your template, everytime a new statement starts an attention sound will be played.
If you want a certain statement to be repeated *n* times just add an x followed by *n*.

Such a statement could look like this: `4x100x4.`
Here the metronome would play 4 bars at 100 bpm and repeat this pattern 4 times.
This simplifies things if you want the attention sound to be played multiple times (for example because you want to be reminded to switch hands), without wanting to change the number of bars or the tempo.

If you want the whole template to be repeated in an infinite loop, just add an *r* at the beginning of the template.

A whole template could look like this: `r2x100.4x100x2.2x120.4x120x2.2x130.4x130x2.`

Note that the template language is rather sensitive. Whitespaces, additional dots, etc. can lead to your template not being created.


## Sources ##

The sounds and most of the graphics used here are not my own. See Info for a list of my sources.
