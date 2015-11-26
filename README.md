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

Rant is a language for procedurally generating text, written in C#. It combines a markup language with functional and imperative programming concepts to deliver a powerful, but easy-to-use tool for adding rich variations to your text. The ultimate goal of Rant is to augment your creativity with the boundless potential of randomness, helping you consider your next great idea as not just a static concept, but a seed for countless possibilities.

[berkin.me/rant](http://berkin.me/rant)

##Features of Rant

* Recursive, weighted branching with customizable selection strategies
* Dictionary queries
* Automation for capitalization, rhyming, and indefinite articles
* Multiple output support
* Richard, an experimental embedded scripting language
* Probability modifiers
* Loops, conditionals, and subroutines
* Package loader for easy resource management
* Unmanaged function exports for use in C/C++ applications *(Windows only, sorry!)*
* Compatible with Unity
* **And a whole lot more!**

##Examples

**Fill in the blanks**
```
<name-male> likes to <verb-transitive> <noun.s> with <pron.poss-male> pet <noun-animal> on <timenoun.s-dayofweek>.
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
* Documentation
* Fixing bugs
* Optimization
* New functions
* New language features
* Improving old language features
* New API features
* Formatting support for other cultures

##Learn Rant
See [berkin.me/rantdocs](http://berkin.me/rantdocs) for full documentation of the API and Rant language, as well as additional example code.

##Support Rant
If you love my work and want to support it by donating, you can do so [here](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&encrypted=-----BEGIN+PKCS7-----MIIHFgYJKoZIhvcNAQcEoIIHBzCCBwMCAQExggEwMIIBLAIBADCBlDCBjjELMAkGA1UEBhMCVVMxCzAJBgNVBAgTAkNBMRYwFAYDVQQHEw1Nb3VudGFpbiBWaWV3MRQwEgYDVQQKEwtQYXlQYWwgSW5jLjETMBEGA1UECxQKbGl2ZV9jZXJ0czERMA8GA1UEAxQIbGl2ZV9hcGkxHDAaBgkqhkiG9w0BCQEWDXJlQHBheXBhbC5jb20CAQAwDQYJKoZIhvcNAQEBBQAEgYCnAa5a%2BlNDRPC3XmQ9m0fiEQcJzDJ0ukikmnDuVGFs%2BrGHX23SXuDeWT8v7FOAPu6Rdipva1soJIjJTUuk0HiEzwPAiSVjkV%2Fj8NSlcbPNnSyHEmiE7%2BDzKpJBGGA4WH8gwbtDUQ%2Be9ILdjUJIZ2KSwcWwbxexk0QP%2BAHKQ0i4xTELMAkGBSsOAwIaBQAwgZMGCSqGSIb3DQEHATAUBggqhkiG9w0DBwQIshT08dOlnw2AcGtKUFBUobeoq2XmJHDzw42kMkNWgad2zWdpmoL75wRaCKYjBDGX1MVw9NE5agB8QJfBdrNYZLZPB2i5lKBA%2BPccoi4c9us%2FSVLoNGffwTlY7dNvP%2F1EP0u%2BU3pX2X8e7JBjjcu%2FrdqyQJ5xJf8vGv%2BgggOHMIIDgzCCAuygAwIBAgIBADANBgkqhkiG9w0BAQUFADCBjjELMAkGA1UEBhMCVVMxCzAJBgNVBAgTAkNBMRYwFAYDVQQHEw1Nb3VudGFpbiBWaWV3MRQwEgYDVQQKEwtQYXlQYWwgSW5jLjETMBEGA1UECxQKbGl2ZV9jZXJ0czERMA8GA1UEAxQIbGl2ZV9hcGkxHDAaBgkqhkiG9w0BCQEWDXJlQHBheXBhbC5jb20wHhcNMDQwMjEzMTAxMzE1WhcNMzUwMjEzMTAxMzE1WjCBjjELMAkGA1UEBhMCVVMxCzAJBgNVBAgTAkNBMRYwFAYDVQQHEw1Nb3VudGFpbiBWaWV3MRQwEgYDVQQKEwtQYXlQYWwgSW5jLjETMBEGA1UECxQKbGl2ZV9jZXJ0czERMA8GA1UEAxQIbGl2ZV9hcGkxHDAaBgkqhkiG9w0BCQEWDXJlQHBheXBhbC5jb20wgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBAMFHTt38RMxLXJyO2SmS%2BNdl72T7oKJ4u4uw%2B6awntALWh03PewmIJuzbALScsTS4sZoS1fKciBGoh11gIfHzylvkdNe%2FhJl66%2FRGqrj5rFb08sAABNTzDTiqqNpJeBsYs%2Fc2aiGozptX2RlnBktH%2BSUNpAajW724Nv2Wvhif6sFAgMBAAGjge4wgeswHQYDVR0OBBYEFJaffLvGbxe9WT9S1wob7BDWZJRrMIG7BgNVHSMEgbMwgbCAFJaffLvGbxe9WT9S1wob7BDWZJRroYGUpIGRMIGOMQswCQYDVQQGEwJVUzELMAkGA1UECBMCQ0ExFjAUBgNVBAcTDU1vdW50YWluIFZpZXcxFDASBgNVBAoTC1BheVBhbCBJbmMuMRMwEQYDVQQLFApsaXZlX2NlcnRzMREwDwYDVQQDFAhsaXZlX2FwaTEcMBoGCSqGSIb3DQEJARYNcmVAcGF5cGFsLmNvbYIBADAMBgNVHRMEBTADAQH%2FMA0GCSqGSIb3DQEBBQUAA4GBAIFfOlaagFrl71%2Bjq6OKidbWFSE%2BQ4FqROvdgIONth%2B8kSK%2F%2FY%2F4ihuE4Ymvzn5ceE3S%2FiBSQQMjyvb%2Bs2TWbQYDwcp129OPIbD9epdr4tJOUNiSojw7BHwYRiPh58S1xGlFgHFXwrEBb3dgNbMUa%2Bu4qectsMAXpVHnD9wIyfmHMYIBmjCCAZYCAQEwgZQwgY4xCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJDQTEWMBQGA1UEBxMNTW91bnRhaW4gVmlldzEUMBIGA1UEChMLUGF5UGFsIEluYy4xEzARBgNVBAsUCmxpdmVfY2VydHMxETAPBgNVBAMUCGxpdmVfYXBpMRwwGgYJKoZIhvcNAQkBFg1yZUBwYXlwYWwuY29tAgEAMAkGBSsOAwIaBQCgXTAYBgkqhkiG9w0BCQMxCwYJKoZIhvcNAQcBMBwGCSqGSIb3DQEJBTEPFw0xNTA2MjYwMzI3MjdaMCMGCSqGSIb3DQEJBDEWBBS8ixZRspLeAGmBXnwP4OjcWr228TANBgkqhkiG9w0BAQEFAASBgHnORUb9B9jxH0olsERVeNBf9hok18rGVNIvIedgiojMKSQb13UsNbN4ys%2BWln4OsOhcNh%2FKo2UthrwEB0gZEVNnD6%2BuDL1ogyFAadA3VNHrni7H7iCk3WrgsxgIynHGV68yAOqbcwp8WizAR%2BlHTaxRaZ2jwQ2O%2FRVI%2FkJ34QGs-----END+PKCS7-----%0A++++++++). Donating isn't required, but it's much appreciated!

##Other projects
If you like Rant, you may also like these other, Rant-related projects:
* [**RIDE**](http://github.com/RantLang/RIDE): The official (and highly WIP) Rant IDE
* [**Rantionary**](http://github.com/TheBerkin/Rantionary): The official Rant dictionary

:squirrel:
