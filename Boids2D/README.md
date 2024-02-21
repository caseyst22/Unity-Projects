# Boids 2D

This is an implementation of an algorithm to simulate animal group/flock behavior based on [Effective leadership and decision-making in animal groups on the move](https://www.nature.com/articles/nature03236?message=remove&free=2), a paper by Iain Couzin. A boid is defined as a bird-like object, and Couzin describes three rules which, when simulated together, can produce behavior similar to that which we observe in birds, fish, and other groups of animals.

1. Separation - boids move away from each other if they are too close
2. Alignment - boids attempt to match the velocity of others near them
3. Cohesion - boids will "jostle for position" by attempting to move towards the center of the group

In my implementation, I attempt to simulate these three behaviors in a 2D environment.

Inspired by [Coding Adventure: Boids](https://www.youtube.com/watch?v=bqtqltqcQhw), a Youtube video by Sebastian Lague.
