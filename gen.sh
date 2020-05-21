#!/bin/bash

mono gplex.exe /unicode SimpleLex.lex && mono gppg.exe /no-lines /gplex SimpleYacc.y
