# GFDecompress
소녀전선 데이터 파싱 프로젝트
-----------------------------------------------
## 개요
* Util
  * DataUtil.cs : 각종 데이터 변환 클래스
  * jsonUtil.cs : 36db에 맞는 json형식으로 변환 클래스
* STC
  * Gun.cs : 5005.stc 변환 클래스 (인형 데이터)
  * Squad.cs: 5006.stc 변환 클래스 (화력지원소대 데이터)
  * Skin.cs: 5048.stc 변환 클래스 (스킨 데이터)
  * Equip.cs: 5038.stc 변환 클래스 (장비 데이터)
  * BattleSkillConfig.cs 5001.stc 변환 클래스 (스킬 데이터)
  * MissionSkillConfig.cs 5046.stc 변환 클래스 (스킬 데이터)
* StcBinaryReader.cs : 복호화 클래스
* Program.cs : 최초실행 클래스
