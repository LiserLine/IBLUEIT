// I Blue It Serial Connection

const unsigned char MPX5010DP_PIN = 2;
const float MPX5010DP_VOLT_OFFSET = 0.2;

int readValue = 0;
float outputVolt = 0;  
float intToVolt = 5.0/1024.0; // https://www.arduino.cc/en/Reference/AnalogRead
float diffPressure = 0;
float flowValue = 0;

bool requestEnabled = false;

const int MEAN_CALCULATOR = 256;

void setup() 
{
	Serial.begin(115200);
}

int i = 0;
void loop()
{
	if (Serial.available() > 0)
	{
		char cmd = Serial.read();
		
		//ECHO
		if (cmd == 'e' || cmd == 'E') 
			Serial.println("echo");
		
		//ENABLE REQUEST
		else if (cmd == 'r' || cmd == 'R')
			requestEnabled = true;

		//DISABLE REQUEST
		else if (cmd == 'f' || cmd == 'F')
			requestEnabled = false;

	}	
	
	//ANSWER
	if(requestEnabled)
	{
		diffPressure = 0.0;
		
		for(i = 0; i < MEAN_CALCULATOR; i++)
		{
			readValue = analogRead(MPX5010DP_PIN);
			outputVolt = (readValue * intToVolt) - MPX5010DP_VOLT_OFFSET;
			diffPressure += TransferFunction(outputVolt) * 1000;
		}
		
		diffPressure /= MEAN_CALCULATOR;

		Serial.println(diffPressure, 3);
	}
}

// Ref: Datasheet MXP5010DP
float TransferFunction(float outputVolt) 
{
	return ((outputVolt / 5.0) + 0.04) / 0.09;
}
