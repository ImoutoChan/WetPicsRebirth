#!/bin/bash

cd /home/WetPicsRebirth/
docker-compose -f docker-compose.yml -f docker-compose.production.yml down
docker-compose -f docker-compose.yml -f docker-compose.production.yml pull wetpicsrebirth
docker-compose -f docker-compose.yml -f docker-compose.production.yml up -d
