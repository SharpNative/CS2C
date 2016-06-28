/* Namespace <TestProject.OtherNameSpace> */
/* Class <TestClass> */
struct class_TestProject_OtherNameSpace_TestClass
{
	int32_t usage_count;
	/* Field: nonStaticTest */
	int32_t field_nonStaticTest;
	/* Field: test */
	int32_t field_test;
	/* Property: testing */
	int32_t prop_testing;
};

struct
{
	/* Static Field: staticTest */
	int32_t staticTest;
} classStatics_TestProject_OtherNameSpace_TestClass;

void classCctor_TestProject_OtherNameSpace_TestClass(void)
{
	classStatics_TestProject_OtherNameSpace_TestClass.staticTest = 4;
}


struct class_TestProject_OtherNameSpace_TestClass* classInit_TestProject_OtherNameSpace_TestClass(void)
{
	struct class_TestProject_OtherNameSpace_TestClass* object = malloc(sizeof(struct class_TestProject_OtherNameSpace_TestClass));
	if(!object)
		return NULL;
	object->usage_count = 1;
	object->field_nonStaticTest = 2;
	object->prop_testing = 10;
	return object;
}

int32_t TestProject_OtherNameSpace_TestClass_testing_getter(struct class_TestProject_OtherNameSpace_TestClass* obj)
{
	return obj->prop_testing;
}
void TestProject_OtherNameSpace_TestClass_testing_setter(struct class_TestProject_OtherNameSpace_TestClass* obj, int32_t value)
{
	obj->prop_testing = value;
}
/* Constructor <TestClass> */
struct class_TestProject_OtherNameSpace_TestClass* TestProject_OtherNameSpace_TestClass_TestClass(struct class_TestProject_OtherNameSpace_TestClass* obj, int32_t test)
{
	Console_WriteLine("we are in the constructor of shit");
	/* Variable aaa = testing */
	int32_t aaa = TestProject_OtherNameSpace_TestClass_testing_getter(obj);
	/* Expression nonStaticTest = 56 + test */
	obj->field_nonStaticTest = 56 + test;
	/* Expression staticTest = test */
	classStatics_TestProject_OtherNameSpace_TestClass.staticTest = test;
	/* Expression this.test = test */
	obj->field_test = test;
	return obj;
}
/* Method <TestPointer> */
void TestProject_OtherNameSpace_TestClass_TestPointer(struct class_TestProject_OtherNameSpace_TestClass* obj)
{
	/* Variable x = 42 */
	int32_t x = 42;
	/* Variable ptr = &x */
	int32_t* ptr = &x;
	/* Variable address = (int*)0xB8000 */
	int32_t* address = (int32_t*)0xB8000;
	/* Expression address[0] = 3 */
	address[0] = 3;
	/* Variable t = 0 */
	int32_t t = 0;
	/* Variable test = (int)(((t << 3) | (t >> 3)) & 22) */
	int32_t test = (int32_t)(((t << 3) | (t >> 3)) & 22);
	test <<= 3;
	test >>= 5;
	test |= 1;
	test >>= 2;
	/* Expression test = ~1337 */
	test = ~1337;
	if(! (test > 3) )
	{
		Console_WriteLine("hisdfqksdlfj");
	}
	else if(test == 8)
	{
		/* Expression t = 4 */
		t = 4;
	}
	else
	{
		Console_WriteLine("else");
	}
	if((address[1] == 3 && x == 88888) && x + 1 < 8 && x != 222 )
	{
		Console_WriteLine("hooray");
	}
}
/* Method <getNonStaticTest> */
int32_t TestProject_OtherNameSpace_TestClass_getNonStaticTest(struct class_TestProject_OtherNameSpace_TestClass* obj)
{
	return obj->field_nonStaticTest;
}
/* Namespace <TestProject> */
/* Class <Program> */
struct class_TestProject_Program
{
	int32_t usage_count;
};

struct
{
} classStatics_TestProject_Program;

void classCctor_TestProject_Program(void)
{
}


struct class_TestProject_Program* classInit_TestProject_Program(void)
{
	struct class_TestProject_Program* object = malloc(sizeof(struct class_TestProject_Program));
	if(!object)
		return NULL;
	object->usage_count = 1;
	return object;
}

/* Static Method <Main> */
void TestProject_Program_Main(char** args)
{
	/* Variable a = 5 */
	int32_t a = 5;
	/* Variable b = 3 */
	int32_t b = 3;
	/* Variable c = a + b */
	int32_t c = a + b;
	/* Expression b = 2 */
	b = 2;
	TestProject_Program_Method2(33, 44, 88);
	/* Variable nein = new TestClass(44) */
	struct class_TestProject_OtherNameSpace_TestClass* nein = TestProject_OtherNameSpace_TestClass_TestClass(classInit_TestProject_OtherNameSpace_TestClass(), 44);
}
/* Static Method <Method2> */
void TestProject_Program_Method2(int32_t a, int32_t b, int32_t c)
{
	Console_WriteLine("Method2!");
	/* Variable teststring = "lo" */
	char* teststring = "lo";
}
