rem ����proto����ļ�
cd py
python gen_templates.py

//cd ..

cd protobuftools
python run.py

rem ���ɵ�cs���·��
set cs_file=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\ProtoExport\output\csharp
set py_file=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\ProtoExport\output\python
set pb_file=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\ProtoExport\output\pb

rem ��Ŀ·��
set project_cs=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\GF_3_1_3_Demo\Assets\GameMain\Scripts\Network\Packet\ProtoGen
set project_py=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\PythonServer\pb
set project_pb=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\GF_3_1_3_Demo\Assets\XLua\Resources\3rd\pb

rem ������Ŀ���ļ���
rd /s /q %project_cs%
rd /s /q %project_py%
rd /s /q %project_pb%

rem ��������Ŀ·��
xcopy /e/s/i %cs_file% %project_cs%	
xcopy /e/s/i %py_file% %project_py%	
xcopy /e/s/i %pb_file% %project_pb%	

pause