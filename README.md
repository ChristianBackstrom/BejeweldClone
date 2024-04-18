# Work Test
This project was made for an assignment in Futuregames game programmer course.

## My Adaptations
For this project I was suppose to make a bejeweld clone with technology and UX/presentation in mind. So first of all decided that I needed to make the game very understandable and easy to use. I did this by using myself of animations and highlights a lot. I went with an event based input system where i decouple the inputs from the system. 

### UX
For the ux part of this work test I went with doing animations and highlights. For me this was very important since they stated in the work test that UX/Presentation would be discussed. With that I did so that when a gem is pressed for the player to move it would become more see through so that it would not blend in with the other gems. I also added animations for when the gems are matched and removed the gems above would fall down with an animation so it is clear that the gems above are now moved down. If I were to add more to this game I would make it so that when the gems are swaped I would play an animation but do to my event system that was not possible. I would also like to add more UI describing points and combos that could be done. A hint button would be very good to have if the player were to be stuck depending if the game is not time based. When making the grid and bejeweld part of this game I always added gizmos in unity to display for the designers where the grid would be placed and the gems coordinates. This was to ease the testing for both myself and others when the time comes to pass it on. 

The gist of the UX for me was to divide all steps into different functions so that I could implement animations and clearly have steps where things can happen. It would be very easy for me to implement Unity events for designers to do things themselves when the things happens.
The code was commented and I used myself of Summary a lot to make the code understandable and since the grid was made independently I made sure the functions were well documented in case another programmer would have need for it. The grid class would be very easy to understand and the functions would have comments under them in the IDE if it were to be used in another script. 

### Technology

#### Grid
When making this system I really wanted to decouple things from the main game so that the code could be reused. That is why I firstly made a grid that was very generic and could be reused for any other purpose. Along with the grid I had a base class called GridElement which would be the base class for the populated class in the grid. When reflecting back upon this I think I would have gone a different route and used interfaces instead since the base class only holds a non-initialized variable and a empty virtual function.![image](https://github.com/ChristianBackstrom/BejeweldClone/assets/54669344/57e5fdde-d750-45ee-bc9e-9113cc489e77)

#### Mover
I also decided to make a Mover class that was used to control the moving of gems and only had a function for it in the game manager. That way I did not need to use any type of input in the game manager but had external classes do that logic instead make it so that the bejeweld game can be easily implemented into any other game. Right now the mover class is connected with a Gem reference and it would be beneficial to change it over to GridElements so that it can easily be repurposed with the grid. ![image](https://github.com/ChristianBackstrom/BejeweldClone/assets/54669344/1f91aa4b-ce22-43f1-bc93-ec431679a85e)

#### Async
To make animations for the game I decided to use async since I like the the control I have over the objects instead of the built-in animation package where I need more assets. With this design I can easily make my own animations and control it all myself. I used this when making the gems fall once a gem line was removed. Once the gems were removed I stored all the different x positions and then iterated over their y positions to make the gems fall down to then instantiate the new gems above. ![image](https://github.com/ChristianBackstrom/BejeweldClone/assets/54669344/c8483e7a-cbf0-442c-9aed-653301ec4ab2)
