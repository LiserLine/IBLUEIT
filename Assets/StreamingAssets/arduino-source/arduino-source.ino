// I Blue It Serial Connection

int readValue = 0;
float outputVolt = 0;  
float intToVolt = 5.0/1024.0; // https://www.arduino.cc/en/Reference/AnalogRead
float diffpress = 0;
float flowValue = 0;

const int meanSelector = 140;

const unsigned char MPX5010DP_PIN = 2;
const float MPX5010DP_OFFSET = 0.2;

void setup() 
{
	Serial.begin(115200);
}

void loop()
{
	if (Serial.available() > 0)
	{
		char cmd = Serial.read();
		
		if (cmd == 'e' || cmd == 'E') //echo
			echoHandler();
		
		else if (cmd == 'r' || cmd == 'R') //req
			requestHandler();
	}

	delay(1); 
}

int i = 0;
void requestHandler()
{
	diffpress = 0.0;
	for(i = 0; i < meanSelector; i++)
	{
		readValue = analogRead(MPX5010DP_PIN);
		outputVolt = readValue * intToVolt;
		outputVolt -= MPX5010DP_OFFSET;
		diffpress += TransferFunction(outputVolt) * 1000;
	}
	diffpress /= meanSelector;
	Serial.println(diffpress, 4);
}

void echoHandler()
{
	Serial.println("echo");
}

// Ref: Datasheet
float TransferFunction(float outputVolt) 
{
	return ((outputVolt / 5.0) + 0.04) / 0.09;
}
