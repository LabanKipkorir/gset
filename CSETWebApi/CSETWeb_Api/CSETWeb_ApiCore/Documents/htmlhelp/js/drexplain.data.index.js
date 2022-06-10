DR_EXPLAIN.namespace( 'DR_EXPLAIN.data_index' );
DR_EXPLAIN.data_index = {

	// index
	DREX_NODE_KEYWORDS: [6,12,143,1,5,23,24,6,17,47,40,161,51,162,152,26,25,166,174,173,163,164,149,88,169,170,171,172,178,146,117,118,133,121,122,123,125,7,5,6,10,8,11,150,9,19,5,17,20,15,12,16,18,119,136,151,152,153,154,53,11,155,26,36,156,37,38,47,40,45,39,157,158,159,25,160,2,1,3,175,165,176,177,178,177,179,180,181,47,93,116,45,34,33,114,39,115,35,30,92,58,41,178,130,41,178,140,4,109,152,36,51,156,152,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,89,214,215,156,24,23,27,19,16,23,18,26,27,167,168,137,176,176,137,177,137,110,47,40,111,137,46,45,36,91,180,181,179,177,260,242,261,262,263,264,265,266,267,114,58,39,115,58,114,39,130,178,182,139,43,42,138,142,138,42,142,145,28,140,4,2,3,34,31,30,32,29,34,33,91,134,94,35,128,126,127,15,52,92,57,61,130,129,131,84,60,62,85,59,65,69,124,63,68,55,50,51,49,15,126,131,53,49,36,51,50,212,211,210,216,189,217,218,219,220,34,33,221,187,206,197,88,222,188,212,211,210,216,189,217,218,219,220,34,33,221,187,206,197,88,222,188,212,211,203,187,189,247,187,188,211,210,189,219,220,206,217,223,224,213,207,205,225,202,200,199,226,227,228,229,230,91,231,232,212,190,198,233,234,192,235,236,237,88,238,222,218,221,239,240,191,194,195,201,37,89,214,212,211,197,194,191,189,220,88,243,90,187,244,200,196,195,240,245,203,204,208,246,247,209,190,199,198,217,187,249,250,251,212,211,216,252,253,239,220,254,222,230,91,189,88,241,255,217,26,37,37,34,38,100,95,120,110,105,31,113,112,107,132,119,106,148,242,260,263,264,261,30,265,267,266,260,261,265,101,260,263,265,101,134,154,11,141,87,44,54,85,72,82,83,79,135,124,63,78,74,86,73,80,21,84,129,21,73,11,44,80,79,78,77,76,74,82,83,84,11,44,141,86,52,68,58,63,124,56,69,70,61,57,67,66,13,50,22,65,59,85,62,60,135,71,17,72,64,127,54,14,51,92,35,102,129,49,36,44,11,144,44,11,63,126,127,128,129,15,120,52,53,36,49,51,50,35,17,57,102,61,213,187,207,202,200,199,198,189,190,212,211,217,228,229,91,212,211,189,217,241,213,187,202,200,221,212,211,189,217,219,187,200,189,238,212,211,202,75,217,233,212,211,224,192,187,188,202,242,189,236,217,232,188,187,248,240,191,89,214,51,50,49,36,26,104,37,156,99,156,37,37,37,186,156,37,188,258,97,101,259,100,96,38,105,94,112,110,111,31,34,33,109,106,38,105,108,107,132,113,17,98,90,99,96,97,103,37,101,110,111,113,31,106,38,105,99,110,111,106,38,105,107,37,101,99,37,89,110,111,106,38,105,99,108,101,112,48,37,101,110,111,106,38,105,99,132,21,74,77,76,75,11,44,74,77,76,75,11,44,21,79,21,77,76,75,11,44,21,77,76,75,11,44,80,81,82,21,81,77,76,11,44,83,21,81,77,76,11,44,11,147,212,138,139,112,183,94,47,45,184,185,36,231,215,256,236,257,36,189,212,37,156,37,156,37,156,37,101,232,188,100,258,37,89,214,259,191,240,100],
	DREX_NODE_KEYWORDS_START: [0,0,0,4,22,28,29,29,30,33,37,37,45,76,79,81,83,83,88,102,104,109,110,112,113,114,144,144,145,148,156,158,160,162,172,173,174,175,175,175,184,188,191,193,194,199,203,203,205,208,208,209,218,218,218,244,244,247,252,270,288,294,340,370,390,391,392,408,408,408,408,408,408,408,417,421,425,425,426,427,448,460,465,500,503,506,512,523,537,543,553,562,574,577,577,577,577,577,577,577,582,582,586,587,588,588,591,593,594,597,605,608,629,639,648,660,669,669,669,676,683,690,698,705,712,714,714,717,717,717,717,718,719,720,721,722,723,723,725,733,735,737,739,745,745,745,752,752,752], //length:= drex.nodes_count,
	DREX_NODE_KEYWORDS_END: [0,0,4,22,28,29,29,30,33,37,37,45,76,79,81,83,83,88,102,104,109,110,112,113,114,144,144,145,148,156,158,160,162,172,173,174,175,175,175,184,188,191,193,194,199,203,203,205,208,208,209,218,218,218,244,244,247,252,270,288,294,340,370,390,391,392,408,408,408,408,408,408,408,417,421,425,425,426,427,448,460,465,500,503,506,512,523,537,543,553,562,574,577,577,577,577,577,577,577,582,582,586,587,588,588,591,593,594,597,605,608,629,639,648,660,669,669,669,676,683,690,698,705,712,714,714,717,717,717,717,718,719,720,721,722,723,723,725,733,735,737,739,745,745,745,752,752,752,752], //length:= drex.nodes_count,

	DREX_KEYWORD_NAMES: ["<NEW KEYWORD>","Disclaimer","DHS","US Government","Advisory","CSET","Introduction","Background","Objectives","Benefits","Limitations","SAL","Overview","Regulatory Basis","Regulations","Framework","Process","Evaluation","Team","Assessment Team","File Protection","General SAL","Question Categories","System Requirements","Installation","Recovery","Assessment","Start Assessment","User Guide","New Assessment","Information","Executive Summary","Assessment Name","Main Window","Main Screen","Standards","Questions","Analysis","Reports","Resource library","Document Library","Help","Keys","Accessibility","Security Assurance Level","Library","Files","Document","Question Detail","Questions Based","Requirements Based","Requirements","Assessment Mode","Mode","NIST SAL","Quick Start","CoR ver 7","Universal Questions","Catalog of Recommendations","NIST SP800-82","NIST SP800-53","Key Questions","NIST SP800-53 App I","CFATS","NERC CIP","NRC Reg Guide 5.71","TSA","TSA Pipeline Sec Guidelines","CAG","Consensus Audit Guidelines","DOD Instruction 8500.2","Confidentiality Level","MAC Level","Levels","Injury","People","On-Site","Off-Site","Hospitalization","Death","Capital Assets","Money","Economic Impact","Environmental Cleanup","FIPS 199","NIST SP800-60","Information Types","SAL Questions","Tools Menu","Analysis Warnings","Print","Text","Source Documents","Help Documents","Comments","Assessment Compliance","Standard Answers Summary","Summary of Results","Overall Results by Subject Area","Results Per Standard","Analysis Questions","Charts","Subject Areas","Top Concerns","Questions Marked for Review","Reports Screen","Report Builder","Site Summary","Site Detail","PDF","DOC","DOCX","Detail Options","Executive Report","Procurement Language","Search","Index","Acronyms","Key Terms","Security","Category","FAQ","Frequently Asked Questions","Video","CNSSI 1253","Issue","Framework Tiers","NIST Framework","Cybersecurity Framework","Tiers","Profile","Framework Profile","Security Plan","Glossary","Contact","CNSS","eMASS","File","Shortcut","Access Keys","About","Special Factors","Keyboard","Terms of Use","C2M2","Hot Key","Initiation Scenarios","SAL Considerations","Parameter","Title Bar","User Qualifications","Site Information","Preparation","Sector","Demographics","Standard","Results","Resource","Protect","Data Recovery","Data Security","System","Procedure","USB","Web Install","Setup","Reinstallation","Subject Matter","Supporting Documentation","File Menu","Multiple Assessments","Overall Summary","Best To Worst","Stand-alone","Enterprise","Register","Import","Export","Menus","Enable Protected Features","Assessment Documents","Parameter Editor","Password","Supplemental","Discovery","Filter","Ranking","Component Diagram","Components","Network Diagram","Line Connectors","Network Analysis","Multiple Services Component","Microsoft Visio","Manage Templates","Manage Template","Manage Layers","Load Templates","Link","Line Thickness","Layers","Label Styles","Label","Inventory","Import Diagram","Host Name","Icons","Head-Tail Indicator","Export Diagram","Grass Marlin","Drawing Area","Diagram Properties","Diagram","Color","Analyze Network","Default Questions","Grid","Properties Window","Ribbon","Symbols","Toolbar","Shape","Zoom","Criticality","Asset Type","IP Address","Subnet Address","Subnet Name","Trusted Lines","Untrusted Lines","Selector","Component Defaults","Component Type","MSC","Multi Select","Object Positioning","Override","Size","Zones","Show/Hide","Network Warnings","Alignment","Multiple","Plotter","Home","Template","Visio","Excel","Warning","Copy","Cut","Delete","Paste","Redo","Undo","Commands","Defaults","Question Information","Components Summary Results","Drill Down Screens","Aggregation","Trend","Combine","Compare","Merge","Alias","Reconciliation","Reconcile"],
	DREX_KEYWORD_CHILD_START: [1,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268],
	DREX_KEYWORD_CHILD_END: [268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268,268]
	
};