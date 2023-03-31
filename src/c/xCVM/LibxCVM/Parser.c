#include "Parser.h"

ExceptableResult xCVM_Parse(ParserConfiguration config, FILE INPUT)
{
	ExceptableResult result = NewER();
	ExceptableResult NEW_SEG = NewSegment();
	if (!HasException(NEW_SEG)) {
		Segment HEAD = NEW_SEG->Content;
		DestoryERWithoutDestoryingContent(NEW_SEG);
	}
	else {
		
	}
	return result;
}
