rem 生成proto输出文件
cd py
python gen_templates.py

//cd ..

cd protobuftools
python run.py

rem 生成的cs输出路径
set cs_file=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\ProtoExport\output\csharp
set py_file=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\ProtoExport\output\python
set pb_file=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\ProtoExport\output\pb

rem 项目路径
set project_cs=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\GF_3_1_3_Demo\Assets\GameMain\Scripts\Network\Packet\ProtoGen
set project_py=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\PythonServer\pb
set project_pb=E:\GithubProjects\GameFramework\GF_Demo_xluaProto\GF_3_1_3_Demo\Assets\XLua\Resources\3rd\pb

rem 清理项目旧文件夹
rd /s /q %project_cs%
rd /s /q %project_py%
rd /s /q %project_pb%

rem 拷贝到项目路径
xcopy /e/s/i %cs_file% %project_cs%	
xcopy /e/s/i %py_file% %project_py%	
xcopy /e/s/i %pb_file% %project_pb%	

pause