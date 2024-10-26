import cv2
import socket
import threading
import warnings
import logging
import json
import struct
import time
import signal
import atexit
from fer import FER

cap = cv2.VideoCapture(0)
detector = FER()

has_face_been_detected_long_enough = 0
threshold = 20

TCP_HOST = '0.0.0.0'
TCP_PORT = 16
SEND_INTERVAL = 1.0

DEBUG = True

tcp_server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
tcp_server_socket.bind((TCP_HOST, TCP_PORT))
tcp_server_socket.listen(5)
print(f"TCP Server listening on {TCP_HOST}:{TCP_PORT}")

detected_emotions = []
detected_emotions_lock = threading.Lock()

def image_processing_loop():
    global has_face_been_detected_long_enough
    while True:
        ret, frame = cap.read()
        if not ret:
            break

        # Preprocess frame and predict (e.g., resize, normalize)
        result = detector.detect_emotions(frame)

        if has_face_been_detected_long_enough > threshold:
            with detected_emotions_lock:
                detected_emotions.clear()
                for face in result:
                    emotions = face['emotions']
                    detected_emotions.append(emotions)

                    if (DEBUG):
                        print(emotions)

        if len(result) > 0:
            has_face_been_detected_long_enough += 1
        else:
            has_face_been_detected_long_enough = 0

        # Draw bounding box around detected faces
        for face in result:
            (x, y, w, h) = face['box']
            cv2.rectangle(frame, (x, y), (x + w, y + h), (255, 0, 0), 2)

        # Display result
        cv2.imshow('Facial Expression', frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

def start_tcp_server():
    while True:
        client_socket, addr = tcp_server_socket.accept()
        print('TCP Connection from:', addr)
        client_handler = threading.Thread(target=handle_tcp_client, args=(client_socket,))
        client_handler.start()

def handle_tcp_client(client_socket):
    try:
        while True:
            # Get the latest detected_emotions
            with detected_emotions_lock:
                current_emotions = detected_emotions.copy()

            # Send results back to the client as JSON
            response = json.dumps({
                "emotions": current_emotions
            }).encode('utf-8')

            response_length = struct.pack("!I", len(response))  # Use network byte order for consistency
            client_socket.sendall(response_length + response)
            logging.debug(f"Sent detection results of size {len(response)} bytes.")
            print(f"Sent detection results of size {len(response)} bytes.")  # Added print statement for debugging

            # Wait for the specified interval
            time.sleep(SEND_INTERVAL)
    except Exception as e:
        logging.error(f"Error handling client: {e}")
    finally:
        client_socket.close()
        logging.debug("Closed connection with client.")

def cleanup():
    cap.release()
    cv2.destroyAllWindows()
    tcp_server_socket.close()
    print("Cleanup done.")

# Register cleanup function to be called on exit
atexit.register(cleanup)

# Register signal handlers for termination signals
signal.signal(signal.SIGINT, lambda sig, frame: exit(0))
signal.signal(signal.SIGTERM, lambda sig, frame: exit(0))

if __name__ == '__main__':
    logging.info("DEBUG MODE " + str(DEBUG))
    tcp_server_thread = threading.Thread(target=start_tcp_server)
    tcp_server_thread.start()
    logging.info("TCP server started.")

    logging.info("Image processing loop started.")
    image_processing_loop()
    logging.info("Image processing loop ended.")