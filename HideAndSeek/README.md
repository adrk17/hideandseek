# HidenSeek

## How to run learning
1. `conda create -n mlagents python=3.9`
2. `conda activate mlagents`
3. `pip install torch==2.3.1+cu118 torchvision torchaudio --extra-index-url https://download.pytorch.org/whl/cu118`
4. `pip install -r requirements`
##### To run learning in unity edit mode
5. `MlAgents-Learn config.yml --run-id run1 --time-scale 10 --torch-device cuda`
1. Start the game

##### To run learning in server build (way faster)
5. Build your game as server build
1. `MlAgents-Learn config.yml --env=<PATH TO EXE> --run-id run1 --torch-device cuda --num-envs=8 --num-areas=16 --results-dir=Results/run1`