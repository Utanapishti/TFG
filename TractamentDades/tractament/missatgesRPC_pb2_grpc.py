# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc

import missatgesRPC_pb2 as missatgesRPC__pb2


class ValorServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.Valor = channel.unary_unary(
                '/TFGProtocol.ValorService/Valor',
                request_serializer=missatgesRPC__pb2.PeticioValor.SerializeToString,
                response_deserializer=missatgesRPC__pb2.RespostaPeticioValor.FromString,
                )


class ValorServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def Valor(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_ValorServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'Valor': grpc.unary_unary_rpc_method_handler(
                    servicer.Valor,
                    request_deserializer=missatgesRPC__pb2.PeticioValor.FromString,
                    response_serializer=missatgesRPC__pb2.RespostaPeticioValor.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'TFGProtocol.ValorService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))


 # This class is part of an EXPERIMENTAL API.
class ValorService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def Valor(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(request, target, '/TFGProtocol.ValorService/Valor',
            missatgesRPC__pb2.PeticioValor.SerializeToString,
            missatgesRPC__pb2.RespostaPeticioValor.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)
