# HidenSeek

## How to run learning
1. `conda create -n mlagents python=3.9`
2. `conda activate mlagents`
1. `pip install -r requirements`
##### To run learning in unity edit mode
4. `MlAgents-Learn config.yml --run-id run1 --time-scale 10 --torch-device cuda`
1. Start the game

##### To run learning in server build (way faster)
4. Build your game as server build
1. `MlAgents-Learn config.yml --env=<PATH TO EXE> --run-id run1 --torch-device cuda --num-envs=8 --num-areas=16 --results-dir=Results/run1`