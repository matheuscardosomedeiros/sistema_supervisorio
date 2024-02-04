#include <Servo.h>
#include <HCSR04.h>

#define TRIGGER 8
#define ECHO 9
#define servo 6

UltraSonicDistanceSensor distanceSensor(TRIGGER, ECHO);
Servo s;
int pos;

void setup() {
  // put your setup code here, to run once:
  s.attach(servo);
  s.write(0);
  Serial.begin(9600);
}

void loop() {
  int distance = 0;
  distance = distanceSensor.measureDistanceCm();
  if ((distance > 0) && (distance <= 30)) {
    Serial.println(distance);
    distance = map(distance, 0, 30, 0, 180);
    s.write(distance);
  }

}
