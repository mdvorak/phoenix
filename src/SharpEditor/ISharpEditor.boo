namespace SharpEditor

import System
import System.ServiceModel

[ServiceContract]
public interface ISharpEditor:
"""WCF contract"""

    [OperationContract]
    def OpenFile(file as string) as void:
        pass
        
    [OperationContract]
    def BringToFront() as void:
        pass

