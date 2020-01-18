// DO NOT MODIFY! CODE GENERATED BY SHARPPROTO.
#ifndef _TESTPROTO_
#define _TESTPROTO_

#include <string>

namespace SharpProto {
class Person {
 public:
  const std::string& name() const { return name_; }
  void set_name(const std::string& val) { name_ = val; }
  std::string* mutable_name() { return &name_; }
  const int id() const { return id_; }
  void set_id(const int val) { id_ = val; }
  int* mutable_id() { return &id_; }
  std::string DebugString() const {
    std::string str;
    str += std::string("name") + " : \"" + name_ + "\"\n";
    str += std::string("id") + " : " + std::to_string(id_) + '\n';
    return str;
  }
  std::string SerializeAsString() const {
    std::string str;
    return str;
  }
 private:
  std::string name_; // = 1
  int id_; // = 2
};


class Group {
 public:
  const std::string& name() const { return name_; }
  void set_name(const std::string& val) { name_ = val; }
  std::string* mutable_name() { return &name_; }
  const Person& member1() const { return member1_; }
  void set_member1(const Person& val) { member1_ = val; }
  Person* mutable_member1() { return &member1_; }
  const Person& member2() const { return member2_; }
  void set_member2(const Person& val) { member2_ = val; }
  Person* mutable_member2() { return &member2_; }
  std::string DebugString() const {
    std::string str;
    str += std::string("name") + " : \"" + name_ + "\"\n";
    str += std::string("member1") + " {\n";
    str += member1_.DebugString();
    str += "}\n";
    str += std::string("member2") + " {\n";
    str += member2_.DebugString();
    str += "}\n";
    return str;
  }
  std::string SerializeAsString() const {
    std::string str;
    return str;
  }
 private:
  std::string name_; // = 1
  Person member1_; // = 2
  Person member2_; // = 3
};


}  // namespace SharpProto


#endif // _TESTPROTO_