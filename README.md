![Manhood logo](http://i.imgur.com/vYActli.png)

Manhood is a templating language designed to be concise and extremely flexible. It is capable of performing a wide variety of generation tasks, including but not limited to:

* Generating HTML, JSON, and other recursive text-based data structures
* Writing randomly generated words, sentences, and even stories
* Creating strong passwords
* Generating character names and other attributes
* Applying filters to strings
* Random picking from uniform and weighted data sets
* Self-generating code
* An ass-ton more... The sky is the limit!

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

See the [wiki](http://github.com/TheBerkin/Manhood/wiki) for language and API documentation.
