# YGO_Searcher
A small application to search Yu-Gi-Oh cards and create decks (exportable to the .ydk format) !

# Usage
- Launch YGO_Searcher.exe
- If you don't see cards, do File->Update Cards from DB
- If you want to use the GOAT format, do Edit->Use GOAT Format
- Don't forget to do File->Save Cards to file if you want a local store of the cards !
- If you want to add cards to the deck, right-click on them
- If you want to remove cards from the deck, right-click on them
- If you want to export the deck, you can do Edit->Export Deck or use the button Export to .ydk
- If you want to use Deck Codes, click the 'Copy Deck Code' button on the right OR do Edit->Copy Deck Code to Clipboard.
- Copy a Deck Code and do Edit->Import Deck Code in order to import the deck into the program (careful to the format !) !

# TODO
- Stop the program of being detected as a virus u_u
- Deck Importing for .ydk format (I'm lazy, I'll do it one day)

# Changelog

1.7
- Added: Deck Codes ! Easily share deck with Deck Codes

1.6
- Bug fixed: Monster 2nd type is ignored
- Bug fixed: Filters are still apllied after returning to 'Any' filters
- Modif: Deck sorting is now correct (that's cool)
- Modif: Choosing Alpha or New sorting now refreshes the list
- Added: Exclusive Pack for GOAT format

1.5
- Added: Edit Menu -> Creation & Export of decks
- Added: About menu (credits :D)
- Added: You can now create decks - Deck list added on the right
- Added: Edit Menu->GOAT Format, cards are stored in another file
- Modif: Scrollbar only sow when needed on the Preview Card section

1.4
- Default Window size adjusted
- Bug fixed: Wyrm & Zombie-type don't show
- Bug fixed: Tokens are recovered from database
- Bug fixed: Forbidden card are not

1.3
- Bug fixed: bad card type during list creation

1.2
- Bug fixed: too much cards ignoring filters
- Added: Check box for archetype research

1.1
- Bug fixed: infinite loop when updating from internet
- Bug fixed: missing cards during a research

1.0
- First version
