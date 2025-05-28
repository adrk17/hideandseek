# HidenSeek

## How to run learning
1. Install conda environment
```
conda create python==3.10.12 --name UnityML -y 
conda activate UnityML
conda install  numpy==1.21.2 -y 
(for /f "delims=" %i in ('where pip ^| findstr UnityML') do @%i install --no-cache-dir mlagents==1.0.0)
(for /f "delims=" %i in ('where pip ^| findstr UnityML') do @%i install torch==2.3.1+cu118 torchvision torch audio --extra-index-url https://download.pytorch.org/whl/cu118)
```
##### To run learning in unity edit mode
1. `MlAgents-Learn config.yml --run-id run1 --time-scale 10 --torch-device cuda`
2. Start the game

##### To run learning in server build (way faster)
5. Build your game as server build
1. `MlAgents-Learn config.yml --env=<PATH TO EXE> --run-id run1 --torch-device cuda --num-envs=8 --num-areas=16 --results-dir=Results/run1`