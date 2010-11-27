namespace SharpEditor

import System
import System.Collections.Generic
import System.IO
import System.Reflection
import System.Xml
import ICSharpCode.TextEditor.Document

public class ResourceSyntaxModeProvider(ISyntaxModeFileProvider):

	private _syntaxModes as List[of SyntaxMode]
	private _base as string
	private _assembly as Assembly
	
	public SyntaxModes as ICollection[of SyntaxMode]:
		get:
			return _syntaxModes
	
	public def constructor(assembly as Assembly, base as string):
		assert assembly is not null
		assert base is not null
		
		_base = base
		_assembly = assembly
		
		syntaxModeStream as Stream = _assembly.GetManifestResourceStream(_base + '.SyntaxModes.xml')
		if syntaxModeStream is not null:
			_syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream)
		else:
			_syntaxModes = List[of SyntaxMode]()

	
	public def GetSyntaxModeFile(syntaxMode as SyntaxMode) as XmlTextReader:
		return XmlTextReader(_assembly.GetManifestResourceStream(_base + '.' + syntaxMode.FileName))

	
	public def UpdateSyntaxModeList():
		pass

