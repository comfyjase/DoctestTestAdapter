#pragma once

class BaseClass
{
protected:
	BaseClass() :
		_number(0)
	{ }

	int _number;
};

class BaseClassWithBracketOnSameLine {
protected:
	BaseClassWithBracketOnSameLine() :
		_number(0)
	{
	}

	int _number;
};

class InheritingClassWithBracketOnSameLine : public BaseClass {

public:
	InheritingClassWithBracketOnSameLine() : BaseClass()
	{}

};

class InheritingClass : public BaseClass
{

public:
	InheritingClass() : BaseClass()
	{
	}

};
