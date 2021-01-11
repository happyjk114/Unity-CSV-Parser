# Unity-CSV-Parser
CSV파일정보를 프리팹에 입력하기 위한 파싱기입니다.

사용하기 전에 확인할것.
1. 데이터 파일의 분류명과 스크립트의 필드 이름이 같아야 합니다.(대소문자 같아야 합니다) (순서는 같지 않아도 됩니다.)
2. 데이터파싱 받을 스크립트는 본 스크립트 안에 있는 IHasPrefabID를 구현해야 합니다.
3. Undo 안됨
4. 타입별 처리 int,long,double,float,string 까지 되어있고 추가 타입이나 커스텀 타입은 함수 아래에 정의해야 합니다.
5. Editor폴더에서 관리되어야 하고 에디터에서만 작동합니다.


실제 사용 예




![파싱1](https://user-images.githubusercontent.com/37136317/104232869-4a195800-5494-11eb-957a-4d9ee7f17ddb.png)

![파싱3](https://user-images.githubusercontent.com/37136317/104234186-64eccc00-5496-11eb-8f92-95cc399bbc98.png)

위에 사진처럼 준비하고

ParseData<Gun>("prefabID","Assets/Data/Gun.csv", "Weapon");

이렇게 쓰면

![파싱2](https://user-images.githubusercontent.com/37136317/104232875-4be31b80-5494-11eb-8ee3-6fbcaebf082f.png)

우왕

