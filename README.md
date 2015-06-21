<p align="center">
<img src="http://i.imgur.com/QB3TNYq.png" alt="Rant logo"></img>
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

##What is Rant?

Rant is a language for procedurally generating text. It combines a markup language with functional and imperative programming concepts to deliver a concise but powerful tool for adding rich variations to your text. The ultimate goal of Rant is to augment human creativity with the boundless potential of randomness, helping content producers consider their next ideas as not just static concepts, but seeds of countless possibilities.

##Features of Rant

* Recursive, weighted branching with customizable selection strategies
* Dictionary queries
* Automation for capitalization, rhyming, and indefinite articles
* Multiple output support
* Object-oriented features
* Probability modifiers
* Loops, conditionals, and subroutines
* Package loader for easy resource management
* Unmanaged function exports for use in C/C++ applications *(compile Rant for x86/x64 to enable this feature)*
* Compatible with Unity
* **And a whole lot more!**

##Examples

**Fill in the blanks**
```
<name-male> likes to <verb-transitive> <noun.plural> with <pron.poss-male> pet <noun-animal> on <timenoun.plural-dayofweek>.
```
```
Alick likes to mount shuttlecocks with his pet bat on Mondays.
```

---

**Generate ten random numbers between 1 and 50 and spell them out**
```
[case:sentence][numfmt:verbal-en][rep:10][sep:\s]{[num:1;50].}
```
```
Four. Ten. Thirteen. Fifteen. Eighteen. Twenty four. Seven. Forty eight. Nineteen. Twenty five.
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

**?????**
```
[rs:16;\N]
{
    [r:50]{([rr])\u2593|([re])\s}
}
```
```
▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓ ▓▓▓▓   ▓    ▓       ▓ ▓          
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓ ▓ ▓▓▓ ▓            ▓         
▓▓▓▓▓▓ ▓▓▓▓▓ ▓ ▓▓▓ ▓ ▓  ▓ ▓▓▓▓ ▓▓ ▓   ▓           
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓ ▓  ▓  ▓    ▓▓   ▓▓  ▓▓         
▓▓▓▓▓▓▓▓▓▓▓ ▓▓▓ ▓ ▓▓ ▓ ▓ ▓  ▓ ▓ ▓▓▓▓   ▓          
▓▓▓▓▓▓▓▓▓▓ ▓▓▓ ▓▓▓▓▓▓ ▓  ▓  ▓▓                    
▓▓▓▓▓▓ ▓▓▓  ▓▓▓  ▓▓ ▓▓ ▓ ▓▓  ▓▓▓ ▓  ▓ ▓ ▓      ▓  
▓▓▓▓▓▓▓▓▓▓▓▓▓▓ ▓    ▓▓  ▓▓ ▓▓▓▓ ▓▓▓  ▓            
▓▓▓▓▓▓▓▓▓▓ ▓▓▓   ▓▓▓▓▓▓▓ ▓▓                 ▓     
▓▓▓▓▓▓▓▓ ▓▓ ▓▓▓▓      ▓ ▓  ▓ ▓  ▓  ▓   ▓  ▓▓      
▓▓▓▓▓▓▓▓▓▓ ▓ ▓ ▓▓ ▓ ▓▓▓▓▓▓▓ ▓      ▓   ▓          
▓▓▓▓▓▓▓▓▓▓ ▓  ▓▓   ▓▓▓   ▓▓▓▓▓     ▓ ▓▓           
▓▓▓▓ ▓▓▓▓▓▓▓ ▓▓▓ ▓  ▓▓▓ ▓    ▓     ▓    ▓         
▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓ ▓▓▓   ▓ ▓▓ ▓▓▓   ▓▓        ▓     
▓▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓  ▓▓▓ ▓▓ ▓▓   ▓      ▓ ▓ ▓        
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓     ▓    ▓       ▓▓▓ ▓      
```


##NuGet
Rant is also available as a [NuGet package](https://www.nuget.org/packages/Rant/). Punch the following into your package manager console and smash the Enter key enthusiastically to get it:
```
PM> Install-Package Rant
```
Or if development builds are your thing:
```
PM> Install-Package Rant -Pre
```

But remember, the latest version will always be available on the repository first.

##License
Rant is provided under [The MIT License](https://github.com/TheBerkin/Rant/blob/master/LICENSE).

##Improve Rant
If there is something you want fixed, added, or changed, feel free to submit an issue/pull request. You are welcome to help with any of the following:
* Documentation (wiki/code)
* Fixing bugs
* Optimization
* New functions
* New language features
* Improving old language features
* New API features
* Formatting support for other languages

We have a [Trello board](https://trello.com/b/NnvgqGha/rant) where we also track progress on new features and bugfixes.

##Learn Rant
See [rantlang.github.io](http://rantlang.github.io) for full documentation of the API and Rant language, as well as additional example code.

##Other projects
If you like Rant, you may also like these other, Rant-related projects:
* [**RIDE**](http://github.com/RantLang/RIDE): The official (and highly WIP) Rant IDE
* [**Rantionary**](http://github.com/TheBerkin/Rantionary): The official Rant dictionary

:squirrel:
