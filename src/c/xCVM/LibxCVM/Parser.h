#pragma once
#include "base.h"

typedef struct LineComId {
	char* startseq;
}LineCommentIdentifier;

typedef struct ClosableComId {
	GenericList Start;
	GenericList End;
}ClosableCommentIdentifier;

typedef struct SegEncId {
	char L;
	char R;
} SegmentEncapsulationIdentifier;

typedef struct parserconf {
	GenericList PredefinedSegmentTemplate;
	GenericList PredefinedSegmentCharacters;
	GenericList SegmentEncapsulationIdentifiers;
	GenericList lineCommentIdentifiers;
	/////////////////////////////
	//ClosableCommentIdentifier//
	/////////////////////////////
	GenericList closableCommentIdentifiers;
}* ParserConfiguration;

ExceptableResult xCVM_Parse(ParserConfiguration config, FILE INPUT);