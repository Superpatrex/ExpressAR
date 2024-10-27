import socket
import pickle
import struct
import time
import json

TCP_HOST = 'localhost'
TCP_PORT = 8081

def receive_data_from_server():
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        client_socket.connect((TCP_HOST, TCP_PORT))
        print("Connected to TCP server at localhost:16")

        while True:
            # Receive the size of the response from the server
            data = b""
            while len(data) < struct.calcsize("!I"):
                packet = client_socket.recv(4096)
                if not packet:
                    break
                data += packet
            if not data:
                break
            packed_msg_size = data[:struct.calcsize("!I")]
            data = data[struct.calcsize("!I"):]
            msg_size = struct.unpack("!I", packed_msg_size)[0]

            # Receive the actual response data from the server
            while len(data) < msg_size:
                packet = client_socket.recv(4096)
                if not packet:
                    break
                data += packet

            # Deserialize the response
            response = json.loads(data.decode('utf-8'))
            print(f"Received Data: {response}")

            # Add a short delay between receiving data
            time.sleep(0.1)
    except Exception as e:
        print(f"Error during TCP connection or data transmission: {e}")
    finally:
        client_socket.close()
        print("TCP connection closed")

if __name__ == "__main__":
    receive_data_from_server()