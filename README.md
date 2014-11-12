<p align="center">
<img src="http://i.imgur.com/TFMydmj.png" alt="Rant logo"></img>
<br/><b><i>Generating text has never been this simple.</i></b>
</p>

**Rant** is a language for adding rich variations to text. It writes what you don't want to, faster.

###Features:

* A powerful dictionary querying system for selecting random words
* Randomized branching
* Printing loops (repeaters) with optional separators
* Automatic capitalization and indefinite article (a/an) formatting
* Multiple outputs
* Insert/modify text in existing output
* Subroutines for easy task repetition
* Self-generating code (metapatterns)
* Arithmetic functionality
* Advanced control flow with flags and comparisons
* Probability modifiers to control frequency of certain elements
* Much, much more...

Need some examples? Behold!

**Generate ten random numbers between 1 and 50 and spell them out using US spelling conventions**
```
[numfmt:verbal-en][rep:10][sep:,\s]{[num:1;50]}
```
```
thirteen, twenty two, thirteen, nineteen, thirty one, thirty four, forty two, twenty six, twelve, forty four
```

---

**Generate 32 random hexadecimal digits grouped into sets of eight**
```
[rep:4][sep:\s]{\8,x}

Alternatively...

[rep:32]{\x[notlast:[nth:8;0;\s]]}
```
```
6fb34d31 42e27a48 5884bce5 bf743ec8
```

---

**Generate random sentences**
```
<name-male> likes to <verb-transitive> <noun.plural> with <pron.poss-male> pet <noun-animal> on <time_noun.plural-dayofweek>.
```
```
Alick likes to mount shuttlecocks with his pet bat on Mondays.
```

---

**Insert text into existing output (backwriting)**
```
The following word is [get:N] characters long: \"[mark:a]<noun-animal|fruit>[mark:b]\"
[send:N;[dist:a;b]]
```
```
The following word is 7 characters long: "kumquat"
```

---

##License
Rant is licensed under the [MIT License](https://github.com/TheBerkin/Rant/blob/master/LICENSE).

##Documentation
See the [wiki](http://github.com/TheBerkin/Rant/wiki) for full documentation of the API and Rant language, as well as additional example code.

##Try Rant
[RantBox - Online Rant Interpreter](http://berkin.me/rantbox)
