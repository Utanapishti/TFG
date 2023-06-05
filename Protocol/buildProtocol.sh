./protoc --proto_path ../Protobufs --csharp_out=. missatges.proto
./protoc --proto_path ../Protobufs  --csharp_out=. --grpc_csharp_out=. missatgesRPC.proto
