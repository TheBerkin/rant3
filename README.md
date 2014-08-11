![Manhood logo](http://i.imgur.com/EaAUnxV.png)

Manhood is a templating engine and scripting language with a concise grammar. It is capable of performing a wide variety of generation tasks, including but not limited to the following:

* Generating HTML, JSON, and other recursive text-based data structures
* Writing randomly generated word, sentences, or even stories
* Creating strong passwords
* Generating character names and other attributes
* Applying filters to strings
* Random picking from uniform and weighted data sets
* Self-generating code
* An ass-ton more... it's limited by your imagination, honestly.

###Data generation

Manhood uses a simple syntax for performing tasks from the mundane to the wildly intricate. The following example generates ten numbers between 1 and 100, separated by commas:
```
[rep 10][sep ,\s]{[num 1/100]}
```
This outputs something like the following:
```
38, 69, 21, 20, 42, 95, 96, 22, 43, 55
```

###Dictionary integration

Want some random words? Not a problem! Manhood makes it as easy with dictionary queries:
```
[caps first]<name> is <verb.ing in transitive> a <noun> <adv>.
```
This results in a large variety of interesting outputs...
```
Jeff is kissing a lion noisily.
```
```
Ponto is deep-frying a leg courageously.
```
```
Abe is probing a toilet bitterly.
```

This is only a very small sliver of Manhood's functionality. To truly understand if it's right for you, try out [the online interpreter](http://berkin.me/manbox)!

If you're still not convinced, check out this [random webpage generator](http://berkin.me) I wrote in Manhood. *(Warning: Potentially NSFW)*

