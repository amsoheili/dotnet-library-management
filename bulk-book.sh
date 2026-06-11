#!/bin/bash

DELAY=0.0001

for i in $(seq 1 1000000); do
    curl -s -o /dev/null -w "%{http_code}" --location 'http://localhost:5225/books' \
        --header 'Content-Type: application/json' \
        --data '{"title": "bulk book", "authorId": "bbb7880d4268401a886876d26c4caca4"}'
    echo " - request $i"
    sleep $DELAY
done
