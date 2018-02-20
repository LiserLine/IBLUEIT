/*
 * Pitaco Serial Connection - MPX5010DP
 * https://github.com/huenato/iblueit
 */
 
#define SAMPLEQUANT 100
 
bool isCalibrated = false;
float calibrationValue = 0.0;
void Calibrate(int sampleQuantity)
{
	calibrationValue = GetSample(sampleQuantity);
	isCalibrated = true;
}

bool isSampling = false;
float diffPressure = 0.0;
int i = 0;
float GetSample(int sampleQuantity)
{
	diffPressure = 0.0;
	
	for(i = 0; i < sampleQuantity; i++)
		diffPressure += voutToPa(digitalToVout(analogRead(A2)));
		
	return diffPressure / sampleQuantity;
}

float Sample(int sampleQuantity)
{
	return GetSample(sampleQuantity) - calibrationValue;
}

void ListenCommand(char cmd)
{
	//ECHO
	if (cmd == 'e' || cmd == 'E') 
		Serial.println("echo");
	
	//ENABLE SAMPLING
	else if (cmd == 'r' || cmd == 'R')
		isSampling = true;
	
	//DISABLE SAMPLING
	else if (cmd == 'f' || cmd == 'F')
	{
		isSampling = false;
		isCalibrated = false;
	}
	
	//RECALIBRATE
	else if (cmd == 'c' || cmd == 'C')
		isCalibrated = false;
}

void setup() {	Serial.begin(115200); }

void loop()
{		
	if(Serial.available() > 0)
		ListenCommand(Serial.read());
	
	if(isSampling && !isCalibrated)
		Calibrate(500);	
	
	if(isSampling && isCalibrated)
		Serial.println(Sample(SAMPLEQUANT));
}

/**
   Sensor Transformations
   Range 0.2V = 0 kPa to 4.7V = 10.0 kPa
   https://github.com/AdamVStephen/gem-water-level-gauge/blob/master/WaterLevelSensor/WaterLevelSensor.ino
*/

const float VCC = 5.0;
const float MAX_KPA = 10.0;
const float COEFF_LIN_KPA = 0.09;
const float COEFF_OFFSET_KPA = 0.04;
// Min Vout 0.2 for standard values above
const float MIN_VOUT = (VCC * COEFF_OFFSET_KPA);
// Max Vout 4.7 for standard values above
const float MAX_VOUT = (VCC * ((COEFF_LIN_KPA * MAX_KPA) + COEFF_OFFSET_KPA));

float voutToKPa(float v) 
{
  return ( v - MIN_VOUT ) / (VCC * COEFF_LIN_KPA);
}

float digitalToVout(long d) 
{
  return (VCC * d) / 1023.0;
}

float voutToPa(float v) 
{
  return 1000.0 * voutToKPa(v);
}

