#!/bin/bash

cd /home/WetPicsRebirth/
docker compose -f docker-compose.yml down
docker compose -f docker-compose.yml pull wetpicsrebirth
docker compose -f docker-compose.yml up -d
