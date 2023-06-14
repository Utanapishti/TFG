docker build . -f ./Generador/Dockerfile -t generador
docker build . -f ./GestorDades/Dockerfile -t gestordades
docker build . -f ./WebAPI/Dockerfile -t webapi
docker build . -f ./Tractamentdades/Dockerfile -t tractament
