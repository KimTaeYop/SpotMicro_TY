# Spot_micro_TY
유니티 ml agent를 활용해서 spot micro 프로젝트에 기여해보겠다. 강화학습으로 4족보행을 수행하는게 목표다.

## ML-Agent
다음 링크를 따라서 MLAgent를 설치한다. https://blog.naver.com/bible20141/222435836643
 - 설치후 ml agent예제들이 들어있는 파일을 작업중인 Asset위치에 복사해 넣는다.
 - 에러가 200여개 뜨는데, 위 링크의 마지막 부분에 에러 해결법이 기록되어있으니 따라서 에러들을 해결해준다.
 
## Spot 훈련모델 돌려보기
기존에는 수동으로 조종할 수 있도록 ml 코드가 빠져있다. 수동조종키는, a,d,q,e,z,c이다. 자세한건 코드 참고.
NNModel 폴더에 있는 모델을 불러와 spot에 넣어준다음 decision requestor를 추가해주면 학습된 모델대로 spot이 동작한다.
추가적인 학습이 필요하다.
