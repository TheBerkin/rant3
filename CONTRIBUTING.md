Thank you for considering contributing to Rant. We are constantly looking for other talented individuals
to help us build and improve on the project. This document outlines the various ways in which you can help.

Before you start, please consider reading our [code of conduct](CODE_OF_CONDUCT.md), by which all 
contributors are expected to abide.

## Testing

We love tests. Tests love us. We like testing all of Rant's features to make sure they do
what we expect them to do. Unfortunately, writing tests for a large project like Rant
takes a lot of time away from development, so this is an awesome way you can help us out.
If you see a feature that doesn't have sufficient test coverage (there are a lot), we would
be overjoyed if you wrote some tests for it! You can find our tests in the `Rant.Tests` directory.
Tests are written using the NUnit framework.

## Bug reports

If you find something in Rant that you think isn't behaving as it should, **please let us know** by
[submitting an issue](https://github.com/TheBerkin/Rant/issues/new). Before you do, please search through
the existing issues to make sure it wasn't reported already. If it was, you can still help out by providing
any additional information about the problem in the issue's comments.

When you submit a bug report, we expect detailed information about the problem. Please do not simply
write us saying "X feature doesn't work". Sure, this tells us that the feature doesn't work, but it
does not tell us _how_ it doesn't work. If you want to help us understand the problem as best as
possible, here is the information we need from you:

* A brief, concise description of what's wrong
* Which version of Rant you're using
* Your OS, OS version, and .NET version
* Your runtime (Mono or .NET?)
* Detailed steps to reproduce the problem
* Any exception(s) / stack traces you got
* Anything else you think we should know

## Feature requests

Lots of people have ideas for features, and a lot of them are good. We welcome your ideas regardless
of whether they're good or bad; even if they're bad, we might be able to figure out a better way
to implement that idea (so it could lead to a good idea anyway!)

When submitting a feature request, we also ask that you use the issue tracker and
prefix your post title with `[Feature Request]`. In your post, please include the following:

* What is it?
* How does it work exactly? Be as detailed as possible. Illustrations are welcome, but optional.
* If it's an API change, provide signatures for all proposed types/methods/properties/etc.
* Tell us why this feature would be useful.

## Pull requests

Pull requests are welcome. However, we have some rules on what we can accept.

We expect code submissions to follow these rules:
* Use K&R indentation with **tabs only**.
* Adhere to Microsoft's [naming guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).
  * The one exception is non-public field identifiers: 
    * Must start with an underscore for easy identification.
    * Must be in camelCase.

Here is a breakdown of the types of content we are able to accept:

### What we CAN accept:

* Bugfixes
* Typo fixes, even to public-facing APIs
* Localization fixes
* Translations of Rant's string resources to new languages
* Improvements on existing features

### What we CANNOT accept:

* Major API changes
* Changes to features not previously discussed
* Stylistic changes to code format, phrasing, etc.
* Unapproved new features -- Submit a feature request first!

## Maintainers

These are the current maintainers of the project:

* Nicholas Fleck ([@TheBerkin](https://github.com/TheBerkin))
* Andrew Rogers ([@cpancake](https://github.com/cpancake))

For other contributors and supporters, see the [credits](CREDITS.md).
