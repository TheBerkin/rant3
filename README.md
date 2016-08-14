<p align="center">
<img src="http://i.imgur.com/Vx7LyRP.png" alt="Rant logo" height="275px" width="275px"></img>
</p>

<p align="center">
  <a href="https://ci.appveyor.com/project/TheBerkin/rant/branch/master/artifacts">
    <img src="https://ci.appveyor.com/api/projects/status/2vn0imlns20n739a/branch/master?svg=true&passingText=Master%20Build%20Passing&pendingText=Master%20Build%20Pending&failingText=Master%20Build%20Failing" alt="Build status">
    </img>
  </a>
  <a href="https://ci.appveyor.com/project/TheBerkin/rant/branch/dev/artifacts">
    <img src="https://ci.appveyor.com/api/projects/status/2vn0imlns20n739a/branch/dev?svg=true&passingText=Dev%20Build%20Passing&pendingText=Dev%20Build%20Pending&failingText=Dev%20Build%20Is%20Kill" alt="Build status">
    </img>
  </a>
</p>

**Rant** is an all-purpose procedural text generation engine.
It has been refined to include a dizzying array of features for handling everything from
the most basic of string generation tasks to advanced dialogue generation,
code templating, automatic formatting, and more.

The goal of the project is to enable creators, especially those in game development,
to automate repetitive writing tasks with a high degree of creative freedom.
With zero cost to you.

Don't just write. Write with Rant.

[berkin.me/rant](http://berkin.me/rant)

## Features

* Recursive, weighted branching with several available selection modes
* Supports queryable dictionaries.
* Tools for automatic capitalization, rhyming, and indefinite articles
* Return multiple outputs at once
* Probability modifiers
* Loops, conditional statements, and subroutines
* Import/Export resources easily with the .rantpkg format
* Unity3D-ready!
* **And that's just the beginning...**

## Examples

Rant is like a templating language but much more awesome. Here are a few examples of how it works.

**Liven up a narrative with a few simple queries in your text.**
```
<name-male> likes to <verb-transitive> <noun.s> with <pron.poss-male> pet <noun-animal> on <timenoun.s-dayofweek>.
```
```
Alick likes to mount shuttlecocks with his pet bat on Mondays.
```

---

**Count to ten and spell it out.**
```
[case:sentence][numfmt:verbal-en][rs:10;\s]{[rn].}
```
```
One. Two. Three. Four. Five. Six. Seven. Eight. Nine. Ten.
```

---

**Write a poem**
```
[rhyme:perfect]
The <noun(1)::&a> <verb.ed(1)-transitive::&a> the <adj::&a> <noun(1)::&a>.
```
```
The bread fed the red head.
```
```
The drug dug the smug plug.
```

---



## NuGet
Rant is also available as a [NuGet package](https://www.nuget.org/packages/Rant/).
Enter this command in your package manager,
and the latest version of Rant will automagically get installed in your project:

```
PM> Install-Package Rant
```

Or if development builds are your thing:

```
PM> Install-Package Rant -Pre
```

## Standard Dictionary
Rant's standard dictionary can be found [**here**](http://github.com/TheBerkin/Rantionary).

## License
Rant is provided under [The MIT License](https://github.com/TheBerkin/Rant/blob/master/LICENSE).

## Improve Rant
If there is something you want fixed, added, or changed, feel free to submit an issue/pull request. I will try to get back to you within a day.

## Learn Rant
See [berkin.me/rantdocs](http://berkin.me/rantdocs) for full documentation of the API and Rant language, as well as additional example code.

## Donate
If you love my work and want to support it by donating, you can do so [here](http://paypal.me/nicholasfleck).
Donations help me afford better software and equipment for my projects, and any amount is very appreciated.
Thank you!
