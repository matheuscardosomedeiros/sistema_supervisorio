#include <Servo.h>
#include <HCSR04.h>

#define TRIGGER 8
#define ECHO 9
#define servo 6
#define buzzer 10
#define LDR A1

UltraSonicDistanceSensor distanceSensor(TRIGGER, ECHO);
Servo s;
int pos;

void setup() {
  pinMode(LDR, INPUT);
  pinMode(buzzer, OUTPUT);
  s.attach(servo);
  s.write(0);
  Serial.begin(9600);
}

void moveServo(char angle) {
  if (angle == '0') {
    s.write(90);
  }
  if (angle == '1') {
    s.write(0);
  }
}

void soundBuzz() {
  tone(buzzer, 441);
}

void disableBuzz() {
  noTone(buzzer);
}

void sendData() {
  Serial.println(analogRead(LDR));
}

void sendDist() {
  int distance = 0;
  distance = distanceSensor.measureDistanceCm();
  Serial.println(distance);
}

void loop() {
  if (Serial.available()) {
    char data = Serial.read();
    if (data == 'a') {
      sendData();
    }
    if (data == 'b') {
      sendDist();
    }
    if (data == 'c') {
      soundBuzz();
    }
    if (data == 'd') {
      disableBuzz();
    }
    moveServo(data);
  }

}

